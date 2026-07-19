# MagicChronicle 仕様・解析索引

このディレクトリはMagicChronicleの仕様正本候補とOmnipotens再解析の最終成果を、BOMなしUTF-8で整理する。

## 企画・仕様

- `feature/product-brief.md`: 企画意図、対象体験、成功条件
- `feature/game-spec.md`: ゲームルールと未決定事項
- `interface/runtime-boundaries.md`: Unity実装の境界
- `data/game-data-contracts.md`: ゲームデータ契約

## 解析結果

- `plan/00-executive-summary.md`: 方向別の要約と優先順位
- `plan/03-ludus-analysis.md`: 遊びの辞書、新規性、遊びの構造
- `plan/04-domain-model.md`: 仕様を基準にしたドメイン定義
- `plan/05-anatomia-analysis.md`: Unity/C#コードグラフ解析
- `plan/06-spec-domain-code-map.md`: 仕様・ドメイン・コード対応
- `plan/07-spec-gaps.md`: 不足・矛盾・未決定事項
- `plan/08-mechanics-economy.md`: メカニクスとゲーム内経済
- `plan/09-aiformat-architecture-review.md`: 文書形式と設計健全性
- `plan/10-ux-review.md`: UX、配信映え、学習・回復
- `plan/11-vitia-marketability.md`: 市場性、グッズ、IP戦略

## 機械可読データ

`data/omnipotens-run-plan.json` が解析範囲、`data/omnipotens-summary.json` が表示用の意味契約、`data/tool-manifest.json` が再現条件を保持する。外部サービスを使うDi工程は今回の範囲外である。
