# Anatomia Unity/C#コード再解析

- 状態: 完了
- 対象コード: `ed9846e62728080798edf34f70237689ae807428`
- Anatomia: `12e3ee10b19c5a2292dce836b75f8f9568aa550e`
- 出力: `../../report/architecture-review.html`、`../data/anatomia-architecture-review.json`

## 学生・初学者向け

ゲームの主要機能は実装されているが、弾の処理、魔法陣の効果、耐久値、画面演出が互いに直接呼び合っている。今のまま機能を増やすと、ある修正が別の挙動を壊しやすい。計算結果を先に作り、その結果をUnityで再生する二段構造へ分けるのが最優先である。

## 高解像度データ

### コードグラフ

| 指標 | 結果 | 解釈 |
|---|---:|---|
| C#ファイル | 120 | EditorスクリプトとUnityTaskManagerを含む |
| 関数 | 424 | MonoBehaviourイベントを含む |
| 解決呼び出し | 347 | 静的に相手を同定できた辺 |
| 未解決呼び出し | 764 | Unityイベント、SDK、動的呼出しを含むため欠陥数ではない |
| 循環 | 12 | うち戦闘コアの7関数循環が最重要 |
| 静的孤立関数 | 129 | Unityライフサイクルの偽陽性が多い |
| 仕様リンクなしファイル | 116 | 仕様IDのコード埋込みがほぼない |

### 主要ホットスポット

| 関数 | 結合 | 循環的複雑度 | 判断 |
|---|---:|---:|---|
| `CardDataBase.GetCardData` | 35 | 2 | 高fan-inはDB窓口として自然。`hold` |
| `TileSlot.DecreaseTimes` | 25 | 4 | 耐久、破壊、効果、演出が集中。`improve` |
| `AttackMagic.ActivateMagic` | 24 | 3 | 効果呼出しと耐久更新を同時に行う。`improve` |
| `AttackMagic.Attack` | 19 | 9 | 移動、衝突、演出、状態を同一コルーチンで処理。`improve` |
| `CardMovement.OnEndDrag` | 17 | 9 | 入力、支払、配置、生成破棄が集中。`improve` |
| `GameManager.Update` | 11 | 7 | フェーズ分岐の入口。状態遷移表へ寄せる。`improve` |

### 最大循環

`Attack → ActivateMagic → Split → ChangeAroundDurability → DecreaseTimes → EffectChangeDurability/EffectSplitAttack → Attack`

これは意図した連鎖を表す一方、計算とUnityコルーチンが同じ循環へ入るため、停止条件・再現性・テストを難しくする。平均的な改善は循環を禁止することではなく、イベント列を作る純粋処理へ閉じ込め、再生側を非循環にすることである。

### ドメイン境界

| 仕様ドメイン | 凝集度 | 判断 |
|---|---:|---|
| Deck & Build | 0.680 | 良好、維持 |
| Narrative Content | 0.571 | 小規模のため保留 |
| Meta Progression | 0.500 | 境界確認 |
| Run Progression | 0.490 | 戦闘Managerとの重複を整理 |
| Combat Encounter | 0.443 | 盤面・効果との境界を整理 |
| Economy & Reward | 0.273 | 報酬表示と経済規則を分離 |
| Board Topology | 0.238 | `AttackMagic`から盤面計算を抽出 |
| Effects & Status | 0.233 | 効果の実行契約を統一 |

汎用`state-machine`は421関数を捕捉し凝集度0.986だが、ほぼ全コードを一箱へ入れた結果なので設計強度として無効である。正直な評価では、仕様ドメインの平均凝集度0.429を基準にする。

### Unity固有の発見

- 画面候補10件を検出したが、ナビゲーション辺は1件。scene/prefab遷移を静的C#だけで復元できないため下限値である。
- `AttackMagic.ChangeAroundDurability`は盤面端の一セルで範囲外になると`continue`ではなく`return`し、残り近傍の処理を打ち切る。
- `RewardManager.Reward`は呼出しごとにスキップボタンへlistenerを追加し、解除が見えない。
- `MapGenerator`は`nextCount - 1`を除数に使うため、1ノード階が許可される場合はゼロ除算相当の境界を持つ。
- コメントの一部はShift_JIS。コード解析は読めるが、共同編集とレビューのためUTF-8統一が望ましい。

## 再現性検証

`review`、`domain-review`、`spec-review`、`screens`を各2回実行し、同一入力でバイト同一出力を確認した。旧安定版の120ファイル、424関数、764未解決呼び出し、12循環も再現した。現行`review`のホットスポット数20は上限変更であり、コード改善を意味しない。

## 不足情報

- Unity scene/prefab、Inspector参照、Addressables/Resourcesの実配線
- 実行トレース、最大同時弾数、フレーム時間、GC割当
- 孤立関数129件のうちUnityから呼ばれるものの正式な除外規則

## 不足実装

- 解決イベント列とUnity再生層の分離
- 盤面端、分岐、破壊、listener重複、1ノード階の回帰テスト
- 仕様IDとコード所有ドメインを結ぶ明示リンク
