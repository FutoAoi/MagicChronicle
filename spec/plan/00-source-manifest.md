# MagicChronicle 解析ソース台帳

- 文書版: 0.1.0
- 取得日時: 2026-07-15 22:55 JST
- 対象リポジトリ: `FutoAoi/MagicChronicle`
- 対象ブランチ: `docs/magicchronicle-analysis-review`
- 対象コミット: `ed9846e62728080798edf34f70237689ae807428`
- 取得方針: 公開Notionを通常のWebページとして取得。Notionワークスペースコネクタは不使用。

## 一次資料

| ID | 資料 | 取得方法 | 用途 |
|---|---|---|---|
| SRC-WEB-001 | [Magic Chronicle 公開Notion](https://mulberry-magazine-1bd.notion.site/Magic-Chronicle-285e692c74fd80b5b588e025135f52ad) | 公開HTMLと、ページが利用する公開Web APIを読み取り | 企画・仕様・世界観・ゲームシステムの正本候補 |
| SRC-PDF-001 | `Magic Chronicle企画書.pdf`（公開Notion添付、10ページ） | 公開ページが発行する一時署名URLから取得し、PyMuPDFでテキスト抽出・全ページ画像確認 | 企画概要、ターゲット、コアループ、開発目標、体制 |
| SRC-SPEC-001 | Notion「仕様書」および画面別子ページ | 公開Web APIを読み取り | 画面遷移、タイトル、拠点、ステージセレクト、インゲーム等 |
| SRC-SYS-001 | Notion「ゲームシステム」 | 公開Web APIを読み取り | 5x5盤面、魔法陣、攻撃、耐久、敵ターン、バフ |
| SRC-WORLD-001 | Notion「ストーリー」 | 公開Web APIを読み取り | 世界観、表設定、裏設定、ボス構造 |
| SRC-CONTENT-001 | Notion「魔法陣案」「敵案」「バフ」「イベント案」 | 公開Web APIを読み取り | コンテンツ案、ビルド軸、状態効果、敵数目標 |
| SRC-CODE-001 | MagicChronicle ソースとUnityアセット | Git commitを固定して読み取り | 実装観測、仕様との配線、コード解析 |
| SRC-LUDUS-001 | Ludus OKF bundle | `b949cfa136fa27de101ace324f99a715f17e6846` | 遊びの語彙、システム、UX、ドメインの参照辞書 |

署名付き添付URLは有効期限を持つため保存しない。個人の連絡先、非公開会話、生テレメトリも成果物へ保存しない。

## 二次資料・解析結果

| ID | 資料 | ピン | 状態 |
|---|---|---|---|
| ANA-AIFORMAT-001 | Ars `Review/MagicChronicle/2026-07-15` | AIFormat `ae8a566700fb108d225e0f2e8f7e42fb61679f7f`、対象コード `4afc6224...` | 現コミットへ差分監査。差は未使用import削除のみ |
| ANA-ANATOMIA-001 | `../../report/architecture-review.html`、`../data/anatomia-architecture-review.json` | Anatomia `2b09de8f79f4c205f6cb797a3135316e764ef56e`、対象 `ed9846e...` | 現コミットへ再実行済み。決定的グラフ指標とヒューリスティックを分離して評価 |
| ANA-LUDUS-001 | Ludus OKF bundle | `b949cfa136fa27de101ace324f99a715f17e6846` | 実行済み。`03-ludus-analysis.md` と `../../report/ludus-analysis.html` |
| ANA-VITIA-001 | Vitia | `SKILL.md` SHA256 `CD4613...A8C2`、score script `9982...64EB` | Luxuria primary 0.768。入力/出力JSONと解釈を保存 |
| ANA-DI-001 | Di / Discutere | `5640fda49e1759a1ce07cfa677e39c5b5379563d` | paper作成済み。今回の実AI議論はユーザ判断で省略 |

## 証拠ラベル

- `source`: 公開企画・仕様に明記された内容。
- `code`: 固定コミットのコードまたはアセットから観測した内容。
- `analysis`: 複数の証拠から導いた分析。
- `hypothesis`: 検証前の提案または仮説。
- `question`: 仕様決定者へ確認が必要な事項。

## 工程状態

| 工程 | 状態 | 備考 |
|---|---|---|
| 仕様・企画初版 | complete | 本ディレクトリの0.1.0 |
| Ludus解析 | complete | 固定コミットのOKF bundleと照合。固有評価は本プロジェクト内にのみ保存 |
| ドメイン整理 | pending | 仕様初版を入力に実施 |
| Anatomia解析 | complete | ビルド済みAnatomiaを現コミットへ再実行。HTML/JSONと解釈資料を保存 |
| 仕様・ドメイン・コード配線 | complete | 8ドメインと主要コード、未実装・不具合・テストを追跡表へ接続 |
| メカニクス・経済解析 | complete | ルールグラフ、内部経済、循環清浄度、複雑度を仕様・コード根拠で暫定評価 |
| AI Format・詳細指標レビュー | complete | AIFormat固定版で差分監査し、現コミットのAnatomia詳細指標と統合 |
| UX | complete | 戦闘HUD、情報階層、tutorial、入力・accessibility、検証指標を提案 |
| Vitia | complete | 公式score scriptでLuxuriaを主軸選定。truth ledgerと実験・倫理監査を保存 |
| Di | accepted-omission | ディスカッションペーパーまで作成し、今回は実AI議論なしで完了扱い |
| 統合HTML | complete | `report/`へ指定の00〜10構成・専門成果物をhash manifest付きでpackaging。Browser目視のみ接続timeoutで未実施 |
