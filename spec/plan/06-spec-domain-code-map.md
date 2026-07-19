# 仕様・ドメイン・コード対応表

- 状態: 完了
- 方針: 仕様ドメインを正本とし、Anatomiaの所属は検証値として扱う

## 学生・初学者向け

企画の言葉とコードの置き場所を対応させる。対応が一つに決まらない箇所は、いまのコードが複数の仕事を持っている合図である。

## 高解像度データ

| 仕様ドメイン | 主要コード | 現在の配線 | 判断 |
|---|---|---|---|
| Run Progression | `GameManager`, `MapManager`, `MapGenerator` | 戦闘フェーズとラン進行が`GameManager`で重複 | `improve`: `RunState`を抽出 |
| Combat Encounter | `GameManager`, `AttackManager`, `CharacterBase` | Unityコルーチンでターンと勝敗を調停 | `improve`: 状態遷移表へ集約 |
| Board Topology | `AttackMagic`, `TileSlot`, `StageManager` | 経路、耐久、効果が循環 | `improve`: 純粋な解決キューへ分離 |
| Deck & Build | `Card`, `CardMovement`, `DeckManager`, `CardDataBase` | 比較的まとまるが入力・支払・生成破棄が混在 | `improve`: 配置コマンド化 |
| Effects & Status | `IEffect`, `IBuff`, `EffectManager`, 各Effect | `OnExcute`から任意のゲーム状態へ直接到達 | `improve`: 効果結果型を統一 |
| Economy & Reward | `WalletManager`, `RewardManager`, `ShopManager` | 表示アニメーションと残高・抽選が混在 | `improve`: 取引と表示を分離 |
| Meta Progression | `CardEncyclopedia`, `UIManager_Camp` | 画面案と図鑑はあるが持越し規則が薄い | 情報待ち |
| Narrative Content | `EventPanelController`, `EventDataBase`, Event各種 | 小規模で境界は比較的明瞭 | `hold` |

## 重要な仕様リンク

| 仕様上の約束 | 実装根拠 | 追跡状態 |
|---|---|---|
| 配置後に自弾、敵弾の順で解決 | `GameManager.UpdateAction:143`、`AttackManager.EnemyTurn:160` | コード確認済み、例外仕様不足 |
| 魔法陣は耐久0で破壊 | `TileSlot.DecreaseTimes:110` | コード確認済み、破壊順不足 |
| 効果が弾道・耐久・状態を変える | `AttackMagic.ActivateMagic:337`、各`IEffect` | コード確認済み、結果契約不足 |
| 報酬からカードを選ぶ | `RewardManager.Reward:22` | 骨格確認、抽選表不足 |
| 分岐マップを進む | `MapGenerator.GenerateMap:6`、`MapManager.MoveTo:50` | 骨格確認、seed/境界不足 |

## 最小テスト配線

1. 仕様例を`ResolutionQueue`入力へ変換する。
2. 同一seed・同一入力から同じイベント列が得られることを検査する。
3. Unityアダプターがイベント列を順番どおり表示することをPlayModeで検査する。
4. UXの予告と振り返りが同じイベント列を使うことを検査する。

## 不足情報

- 仕様項目へ付ける安定IDと責任者
- scene/prefab/ScriptableObjectの対応
- 8ドメインの変更境界と公開API

## 不足実装

- 仕様IDをコード・テストへ結ぶメタデータ
- ドメインごとのUnity非依存テスト
- 予告、実行、リプレイを同じイベント列へ接続するアダプター
