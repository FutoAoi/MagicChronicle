# MagicChronicle 解析ソース台帳

- 状態: 完了
- 分類: public
- 対象リポジトリ: `FutoAoi/MagicChronicle`
- コード基準: `ed9846e62728080798edf34f70237689ae807428`
- 出力文字コード: BOMなしUTF-8

## 入力ゲート

| 項目 | 結果 |
|---|---|
| フェーズ | source-read |
| 状態 | passed |
| 検査ファイル | 1,053 |
| 検査テキスト | 896 |
| バイナリ除外 | 157 |
| 除外ディレクトリ | 0 |
| 大容量上限 | 3 MiB |
| 文字コード | UTF-8を優先し、妥当な日本語Shift_JISだけを読み取り時に復号 |

元ソースの一部はShift_JISだが、解析成果物へ原文コメントを転記せず、すべてUTF-8で記録した。

## 一次資料

| ID | 資料 | 用途 |
|---|---|---|
| SRC-WEB-001 | [Magic Chronicle 公開Notion](https://mulberry-magazine-1bd.notion.site/Magic-Chronicle-285e692c74fd80b5b588e025135f52ad) | 企画・仕様・世界観の正本候補 |
| SRC-PDF-001 | 公開Notion添付「Magic Chronicle企画書.pdf」 | 企画概要、対象、コアループ、開発目標 |
| SRC-CODE-001 | 固定コミットのUnityソース・アセット | 実装、コードグラフ、仕様配線 |
| SRC-LUDUS-001 | Ludus公開OKF辞書 `b949cfa...` | 遊びの語彙と新規性比較 |
| SRC-VITIA-001 | Vitia `3fa33c9e...` | UX・市場性のラベル中立監査 |

署名付き添付URL、個人情報、非公開会話、生テレメトリは保存していない。今回の作業で外部AIサービスへプロジェクトデータを送信していない。

## 証拠ラベル

- `source`: 公開企画・仕様に明記。
- `code`: 固定コミットから観察。
- `analysis`: 複数証拠から導いた解釈。
- `hypothesis`: 実験前の予測。
- `question`: 決定者または実測が必要。

## 工程状態

| 工程 | 状態 | 成果 |
|---|---|---|
| 仕様基準化 | complete | `feature/` |
| Ludus | complete | `03-ludus-analysis.md` |
| ドメイン | complete | `04-domain-model.md` |
| Anatomia | complete | `05-anatomia-analysis.md`、HTML/JSON |
| 仕様追跡 | complete | `06-spec-domain-code-map.md`、`07-spec-gaps.md` |
| メカニクス・経済 | complete | `08-mechanics-economy.md` |
| AIFormat・設計 | complete | `09-aiformat-architecture-review.md` |
| UX | complete | `10-ux-review.md` |
| Vitia市場性 | complete | `11-vitia-marketability.md` |
| Di | not-requested | 外部サービス工程は今回選択していない |
| HTML | complete | `report/omnipotens-final.html`、11ステージHTML、安定JSON |

## 不足情報

- 公開Notion各子ページの版管理と正式な仕様承認日
- Unity scene/prefabと実行時アセットの完全な参照台帳

## 不足実装

- 仕様版とコードcommitを自動照合するrelease receipt
