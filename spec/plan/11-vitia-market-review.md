# Vitia マーケティング・セールスポイント分析

- 文書版: 0.1.0
- 採点入力: `../data/vitia-input.json`
- 採点結果: `../data/vitia-output.json`
- 用途: TGS 2026試遊、Steam store/trailerを想定した仮説

## 1. Truth ledger

### Verified

- 5x5盤面へ魔法陣を配置し、弾の方向、分岐、効果、耐久を操作する企画である。
- プレイヤー弾の後に敵弾が同じ盤面・魔法陣を利用する。
- 耐久0で魔法陣が消え、後続弾の経路が変わる。
- deckbuilder、puzzle、rogueliteの三層を持つ。
- 企画書は18〜35歳のroguelike fan、PC/iOS/Android、一人用を想定する。
- TGS 2026出展を目標とし、動作するUnityコードと画面案が存在する。

### Assumed / hypothesis

- 当面の獲得地点はTGS boothとSteam wishlistである。
- 連鎖の視覚演出は10〜15秒の動画でも固有価値を伝えられる。
- roguelike/deckbuilder経験者は、数値synergyより空間的に見えるsynergyへ新規性を感じる。
- TGS版ではPCを第一gateとし、短い試遊へ絞る。

### Unknown

- 価格、販売日、publisher、Steam page、demo配布条件。
- 実プレイ映像の品質、完成asset、音、frame rate、実機build。
- playtestによる爽快感、理解度、継続率、wishlist転換。
- 競合作品との直接比較、検索需要、市場規模。
- 拠点ガチャ、課金、恒久通貨を製品へ含めるか。

## 2. Diagnosis

### Objective

TGS/短尺動画で、適合するプレイヤーに「この連鎖を自分で作ってみたい」と思わせ、試遊完了またはSteam wishlistへつなげる。

### Bottleneck

`roguelike puzzle deckbuilder`という分類だけでは混雑した市場で固有性が出ない。一方、本当の差別化である「敵も同じ魔法陣を使う」「耐久で解決途中に盤面が変わる」は文章だけだと複雑である。**理解より先に、短い体験episodeとして見せる**必要がある。

### Audience context

対象仮説は、Slay the Spire等のbuild構築と、盤面/自動解決の先読みを好む成人player。短い会場接触では説明を読む余裕がなく、最初の一手から結果までの距離が重要になる。

## 3. Domain selection

| Domain | score | coverage | 判定 |
|---|---:|---:|---|
| Luxuria | **0.768** | 1.00 | **Primary** — 連鎖の視覚・音・即時体験をdemoで届ける |
| Gula | 0.597 | 1.00 | 不採用 — primaryとの差0.171。繰返し価値はmeta仕様/retention証拠がない |
| Acedia | 0.483 | 0.80 | marketing主軸ではなく、試遊activation friction改善に使う |
| Superbia | 0.447 | 1.00 | 閾値未満。masteryは適合するが公開signal/proofが弱い |
| Invidia | 0.345 | 0.30 | peer比較の証拠なし |
| Avaritia | 0.213 | 0.10 | 価格・価値証拠なし |
| Ira | 0.000 | 0.00 | blocked goal訴求は不要 |

Primaryは**Luxuria（anticipated experience）**。ここでの意味は性的・人格的な分類ではなく、商品の価値が感覚的な実演と即時feedbackで最も伝わるというroutingである。

Gulaを副軸にしない理由は、`無限に遊べる`、多様性、collectionをまだ証明できないためである。現時点でretentionを強調すると、未完成のmeta progressionを誇張し、compulsion寄りの表現にもなり得る。

## セールスポイント評価

| 優先 | セールスポイント | 真実との接続 | 見せ方 |
|---:|---|---|---|
| S | **敵もあなたの魔法陣を使う共有盤面** | 企画・実装の中心 | 同じ盤面で自弾成功→敵弾が逆利用する15秒clip |
| S | **耐久で連鎖の途中に道が変わる** | 本作固有の時系列puzzle | 一枚が砕け後続弾が別経路へ抜けるbefore/after |
| S | **一手が多段連鎖へ爆発する** | 企画conceptと一致 | 入力1回→発火番号→大きなhitの音/画面feedback |
| A | deck synergyを盤面で「見える形」に組む | deck + boardの接続 | card三枚が一本のrouteになる短いbuild story |
| A | 強い装置ほど敵にも危険なrisk/reward | 共有盤面から導出 | 最大damage案と安全案をside-by-side |
| B | rogueliteでbuildを育てる | room/reward案あり | meta/varietyが実装・計測後に昇格 |
| C | 古代遺跡×幾何学魔法技術 | art direction案 | 最終asset品質が整ってから補強 |

### 現時点で使わない主張

- 「無限に遊べる」— seed、meta、content量、retentionの証拠がない。
- 「PC/iOS/Android対応」— 実機release gateがない。
- 「戦略は無限大」「毎回全く違う」— 定量根拠がない。
- 「誰でも簡単」— ルールcomplexityとtutorial未検証。
- ガチャ、希少性、期間限定 — 仕様・法務・倫理・確率が未定。

## 4. Strategy card

**Audience hypothesis:** build構築と先読みを好む成人playerは、カードの数値説明より、自分の一手が盤面を変形させて大連鎖になる短い実演に反応する。

**Bottleneck:** 共有盤面と時系列変化を文章で理解する前に離脱する。

**Primary domain / job:** Luxuria — 最初の10〜15秒で「置く→連鎖→砕ける→敵弾で反転」のepisodeを体験させる。

**Verified feature → outcome → mechanism:** 敵味方が同じ魔法陣を発火し耐久で消滅する → 一手が火力とriskを同時に作る → 予測線と連鎖演出を含む短いplayable demoでmental simulationを起こす。

**Proposition:**

> その一手は、最強の連鎖にも、敵の罠にもなる。5×5の魔法陣ローグライト。

**Message variants:**

- `置いた魔法陣を、敵の弾まで使ってくる。`
- `道を組め。連鎖させろ。壊れる順まで読み切れ。`
- `カードのコンボを、盤面に描く。`

**Proof:** 実gameplayの未編集15秒、入力から最終結果までの一続き。自/敵弾、発火順、耐久が読めるUIを含める。

**CTA:** `3分で、一本の連鎖を完成させる`。試遊後は任意の`Steamでフォロー / wishlist`。拒否・離脱を容易にする。

**Channel and moment:** TGS boothのattract loop、試遊待機画面、Steam trailer冒頭、短尺SNS。長いworld説明は後段。

**Boundary condition:** previewが読めず結果が偶然に見えるbuild、未実装platform、未検証の無限性は訴求に使わない。

## 5. Experiment

### TGS/online動画テスト

- **Control:** genre、5x5盤面、deckbuildingを事実として説明する静止/通常動画。
- **Treatment:** 同じofferで、共有盤面の15秒episode + 上記proposition。価格や特典は変えない。
- **対象:** 18歳以上、roguelike/deckbuilderへ関心を自己申告した来場者/視聴者。未成年や脆弱性によるtargetingをしない。
- **Primary metric:** qualified demo start率。Steam上ではtrailer 15秒到達→wishlist率。
- **Guardrail:** demo後の「映像どおりのgameplayだった」trust rating、途中離脱、混乱率、誤期待自由記述。
- **補助:** time-to-first-value、3連鎖達成率、「敵も使う」を説明できる割合。
- **Stopping rule:** 各cell最低200 qualified exposureを目安に事前固定し、会場日/導線差を記録。無作為割当できない会場比較は因果と呼ばない。
- **Disconfirming result:** start/wishlistが上がっても、因果理解が下がる、trustが悪化する、実playの早期離脱が増える場合は不採用。

### Copy test

- A: `カードのコンボを、盤面に描く。`
- B: `その一手は、最強の連鎖にも、敵の罠にもなる。`
- 主指標: 適合者のdemo start。
- Guardrail: 内容理解quizと誤認率。

## 6. Ethics check

- blocked conditionは現入力にない。七domainは人の人格診断に使っていない。
- fake scarcity、countdown、人気捏造、shame、hidden defaultを使わない。
- `無限に遊べる`や対応platformを、証拠が揃う前に事実として広告しない。
- Gula的なrepeat訴求、ガチャ、variable rewardはretentionだけで最適化せず、疲労、後悔、過剰利用、停止容易性をguardrailにする。
- demo/wishlistは任意で、decline、途中退出、後から解除を容易にする。
- gameplay映像は実buildから取り、演出用mockなら明示する。
- 取得dataは必要最小限にし、protected/sensitive traitを推定しない。

## 提案の要点

売るべきものは「カードが多い」ことでも「無限」でもなく、**自分で描いた魔法陣が、時間と敵の介入で予想外の連鎖へ変わる瞬間**である。まずその瞬間が読めるUXと実映像を作り、短いtrialで証明する。
