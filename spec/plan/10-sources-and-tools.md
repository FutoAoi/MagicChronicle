# 10. 引用元・使用ツール

## 一次資料

- Magic Chronicle公開Notion: 企画、仕様、game system、story、魔法陣、敵、buff、event案。
- 公開Notion添付`Magic Chronicle企画書.pdf`全10ページ。
- MagicChronicle repository commit `ed9846e62728080798edf34f70237689ae807428`。

公開Notionはユーザ指定どおり通常のWebページとbrowser-loaded public resourceとして取得した。Notion workspace connectorは使用していない。一時署名URL、個人情報、秘密情報は保存していない。

## 使用ツール

| Tool | Pin / hash | 用途 |
|---|---|---|
| Ludus | `b949cfa136fa27de101ace324f99a715f17e6846` | 遊びの辞書、genre/system/domain照合 |
| Anatomia | `2b09de8f79f4c205f6cb797a3135316e764ef56e` | code graph、cycle、complexity、domain指標 |
| AI Format | `ae8a566700fb108d225e0f2e8f7e42fb61679f7f` | 21観点のgame review形式 |
| Vitia | skill SHA256 `CD4613...A8C2` | market mechanism routingと倫理監査 |
| Vitia scorer | SHA256 `9982...64EB` | 7 domainの決定的採点 |
| Discutere | `5640fda49e1759a1ce07cfa677e39c5b5379563d` | discussion paper形式の参照。実議論は省略 |
| Omnipotens | local reusable skill | 工程統合、HTML packaging、hash manifest |

## 証拠ラベル

- `source`: 公開企画・仕様に明記。
- `code`: 固定commitのcode/assetから観測。
- `analysis`: 複数証拠から導出。
- `hypothesis`: 未検証の提案。
- `question`: 仕様決定者の承認が必要。

## 制限

- Unity compile、EditMode/PlayMode test、実機build、実play、性能計測は未実施。
- 市場規模、競合比較、価格、wishlist/retentionの実測は未取得。
- Di実AI議論は今回省略。
- 最終HTMLはEdge headless（1440×1100）で目視確認済み。UTF-8、hash、local link、決定性も静的検証済み。

完全なURL、取得時刻、tool status、全hashは`00-source-manifest.md`、`../data/tool-manifest.json`、`report/omnipotens-final.manifest.json`を正本とする。
