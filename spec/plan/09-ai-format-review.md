# MagicChronicle AI Format統合レビュー

| 項目 | 値 |
|---|---|
| 対象 | `FutoAoi/MagicChronicle` |
| 対象コミット | `ed9846e62728080798edf34f70237689ae807428` |
| 基準日 | 2026-07-15 |
| AI Format | `ae8a566700fb108d225e0f2e8f7e42fb61679f7f` |
| サービス分類 | `game`（Unityクライアント、バックエンドなし） |
| Anatomia | 120 files / 424 functions / design strength 59.4 |

## 結論

総合評価は**B**、Critical 0、High 9。ScriptableObjectによるデータ分離、魔法・ダメージ表示のobject pool、UI/Effect interfaceの導入は良い。一方、回復不能、exhaustカードの再投入、空山札参照というコアループ停止級の不具合があり、自動テスト、run初期化、clean build、ライセンス、プラットフォーム準備も不足している。**現状はリリース保留**である。

既存AI Formatレビューは一つ前の`4afc6224...`を対象としていた。現コミット`ed9846e...`との差分は`DeckPanelManager.cs`の未使用`NUnit` import削除のみであり、`CODE-011`（Low）は解消済み。他の指摘に影響するコード差分はないため、High件数とカテゴリ評価を維持した。

## 評価一覧

| # | 観点 | 評価 | High | 今回の仕様・構造解析による補足 |
|---:|---|:---:|---:|---|
| 1 | 設計強度 | B | 0 | Anatomia 59.4。循環健全性0、God Class健全性45.8 |
| 2 | 設計思想の一貫性 | B | 0 | データ駆動とManager直書き、serialized設定とhard-codeが混在 |
| 3 | モジュール分割度 | B | 0 | Battle/Boss UI重複、クラスmodularity 0.250 |
| 4 | コード品質 | C | 3 | 回復、カードゾーン、空山札の実害 |
| 5 | コード脆弱性 | B | 0 | 攻撃経路なし。Unity実行・動的検証は未実施 |
| 6 | CI/CD・supply chain | B | 0 | 依存inventory、provenance、clean build gateなし |
| 7 | テスト戦略 | C | 1 | コア戦闘・deck・economy・data integrityの自動テストなし |
| 8 | ライセンス | C | 1 | `LICENSE` / `THIRD_PARTY_NOTICES`不在 |
| 9 | ドキュメント | B | 0 | 今回spec初版を追加。ただしsetup/data/testの実装同期はこれから |
| 10 | LLMセキュリティ | N/A | 0 | LLM機能なし |
| 11 | AI生成コード検収 | B | 0 | 由来・検収方針なし |
| 12 | クライアント信頼境界 | A | 0 | オフライン単体、外部権威値なし |
| 13 | チート対策 | B | 0 | ランキングなし。要件文書なし |
| 14 | セーブ・課金保護 | A | 0 | 永続save/課金なし。将来導入時は再評価 |
| 15 | マルチプレイ | N/A | 0 | オンライン要素なし |
| 16 | セーブデータ設計 | C | 1 | run reset/carry-over境界なし |
| 17 | アセット管理 | C | 1 | CRIWARE/StreamingAssets等の復元手順なし |
| 18 | データ駆動設計 | C | 1 | authoring validatorなし、不正Event IDでsoftlock |
| 19 | パフォーマンス | B | 0 | pool/60fps設定はあるが計測予算なし |
| 20 | プラットフォーム互換 | C | 1 | Mouse依存、safe area、product ID、build gate不足 |
| 21 | accessibility/localization | B | 0 | 文字列、UI scale、remap、色以外の識別が未整備 |

## High指摘

| ID | 場所 | 事実と影響 | 修正の受け入れ条件 |
|---|---|---|---|
| CODE-001 | `CharacterBase.cs:80` | `Mathf.Clamp`戻り値を捨て、回復効果でHPが変わらない | 0/通常/超過回復のtestが通る |
| CODE-003 | `UIManager_Battle.cs:115-125`, `UIManager_Boss.cs:113-123` | exhaust/removedカードを山札へ再投入 | `RunDeckState`でゾーンを一意化し再構築testが通る |
| CODE-015 | 両UIManagerの`DrawCard` | 再構築後も山札0で`DeckCard[0]`を読む | no-cardを正常結果にし、全札が手札/除外でも停止しない |
| QUALITY-001 | `Assets`, `Packages` | 戦闘、deck、報酬、data integrityの自動testなし | Unity非依存rule test + Edit/PlayMode smoke + CI |
| QUALITY-002 | ルート | 配布権利と帰属を集約確認できない | license inventoryと`THIRD_PARTY_NOTICES`を承認 |
| GAMEIMPL-001 | Deck/Wallet/Game Manager | sceneを跨ぐ状態の新規run resetがない | `StartNewRun/Resume/EndRun`で全状態を一括管理 |
| GAMEIMPL-002 | `.gitignore`, CRI audio | clean cloneでvendor/runtime assetを復元できない | version/checksum/取得/配置/licenseを固定しCI build |
| GAMEIMPL-003 | Event/Data/Map | 将来の不正Event IDでblank panel softlock | pre-build validator + runtime fallback +復帰test |
| GAMEQUALITY-001 | Input/UI/ProjectSettings | `Mouse.current` null、safe area、製品設定、対象build gate不足 | PC/iOS/Androidの優先gateごとに端末smokeを通す |

## Anatomia詳細レビューの統合

### 孤立関数

static fan-in 0は129件。ただしUnity lifecycle、EventSystem、Editor callback、Inspector/scene UnityEventが多数を占める。削除数として扱わず、`GameManager.ChangePlayerType`、`GivePlayerBuffData`、旧`AudioManager.PlayBGM`、CardEncyclopedia filter/sort等をscene/prefab・PlayMode到達まで確認する。`RewardManager.RewardSkip`は静的には孤立だが`AddListener`で使用中であり削除不可。

### 複雑度

- 最大cyclomatic: `AttackMagic.Attack` 9、`CardMovement.OnEndDrag` 9。
- 最大級fan-out: `TileSlot.DecreaseTimes` 23、`AttackMagic.ActivateMagic` 22。
- `GameManager.Update`はcyclomatic 7、cross-domain depth 10、shared-state fan-in 4。

極端な単一関数より、盤面・効果・UI・Managerの相互呼び出しによる分散複雑度が主要リスクである。

### 設計強度

59.4/100。componentはdomain coverage 99.3、cohesion 98.6、cycle health 0、directory modularity 53.2、spec linkage 0、God Class health 45.8。coverage/cohesionは421要素が汎用`state-machine`へ集中した結果なので、良好値を過大評価しない。

### ドメイン貧血度

自動値はrisk 18だが、ドメイン分類が一領域へ潰れているため意味上の貧血を捉えない。人手モデルではカード定義と各Dataは存在する一方、カード状態遷移、解決順、run lifecycle、取引不変条件をManager/UIが所有している。`RunDeckState`、`ResolutionQueue`、`RunState`、`RewardTransaction`を抽出する。

### God Class

review candidateは`UIManager_Battle` 54.2、`UIManager_Boss` 54.2、`GameManager` 50.7。70以上のcritical候補はない。二つのUIManagerは同型メソッドと同一不具合を持つため、共通Presenter/HandControllerへの集約を最初に行う。

## 仕様入力で追加された判断

1. 既存Highのdeck不具合は局所修正だけでなく、未定義のカードゾーン契約が根因である。
2. 戦闘循環は本作の連鎖メカニクスを直接表すため、単純にcallを切るのではなく決定的`ResolutionQueue`へ変換する。
3. `Mouse.current`問題は企画のPC/iOS/Android同時記載と直結する。TGS向けPC gateとモバイルgateを分離する。
4. data integrity validatorはイベントsoftlockだけでなく、カード、buff、enemy、stage、mapのID/参照を一括検査する。
5. 「無限に遊べる」を品質要件へ変換し、run/profile reset、seed、横方向解放、履歴を仕様化する。

## リリースゲート

### Gate 1 — コアループ修復

- CODE-001/003/015を修正。
- カード保存則、空山札、回復、耐久端処理、run resetの自動test。
- Battle/Bossの共通カード操作。

### Gate 2 — 決定性とデータ健全性

- `ResolutionQueue`または同等の処理順正本。
- 同じseed・入力から同じイベント列。
- ScriptableObject ID、foreign key、必須参照、rangeのpre-build validator。

### Gate 3 — 再現ビルド

- clean cloneからUnity 6000.3.8f1 compile。
- vendor assetのversion/checksum/配置/license。
- 主要scene smokeと対象プラットフォームbuild。

### Gate 4 — TGS試遊品質

- tutorial、敵意図、射線/発火順preview、safe input。
- 20分等の試遊範囲と成功KPIを固定。
- frame time、メモリ、連鎖演出時間の実測。

## 未実施

Unity Editor compile、EditMode/PlayMode test、実機build、実プレイ、性能計測、動的脆弱性scanは未実施。サービス停止を成功扱いせず、静的レビューと文書化の範囲だけを完了とする。

## 最終判断

**Hold / 改修後再レビュー**。まずコアループの3 Highとrun resetを直し、解決順・カードゾーンのテストを通す。その後にコンテンツ追加、モバイル対応、メタ進行へ進む。
