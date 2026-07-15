# MagicChronicle ディスカッションペーパー

- Paper version: 0.1.0
- Date: 2026-07-15
- Intended engine: Discutere headless discussion
- Evidence policy: `source / code / analysis / hypothesis / unknown`を区別し、未実測を事実扱いしない

## Theme

MagicChronicleは、共有盤面・耐久消滅・deck構築を組み合わせることで「面白く奥深く、売れるゲーム」になり得るか。TGS 2026へ向けて、何を残し、何を削り、何を最初に改善すべきか。

## 議論する三問

1. **面白さ/奥深さ** — 共有盤面と時系列連鎖は、理解可能な深さになるか、それとも予測不能な複雑さになるか。
2. **売れるか** — 競争の激しいroguelike/deckbuilder市場で、短時間に固有価値を証明できるか。
3. **どう改善するか** — TGSまでの限られた期間で、bug、rule、UX、content、marketingのどれを優先すべきか。

## 1. 企画・仕様の要約

- `source`: 一人用roguelike puzzle。5x5盤面へ手札の魔法陣をcostで配置する。
- `source`: 自弾が左から進み、接触した魔法陣が方向変更、分岐、damage、状態等を発火する。
- `source`: 発火で耐久が減り、0で消滅。後続弾は変化後の盤面を通る。
- `source`: player攻撃後、敵弾も同じ魔法陣を発火する。
- `source`: 企画conceptは「ピースがはまった時の爆発的な爽快感」「無限に遊べる」。TGS 2026出展を目標とする。
- `source`: deckbuilding、分岐map、戦闘/event/shop/鍛冶/elite/boss、三つのbuild軸を想定する。
- `unknown`: 完全な解決順、card zone、buff式、run reset、economy値、meta progression、platform gate。

## 2. Ludus解析の要約

- `analysis`: 遊びは盤面puzzle、deckbuilder、roguelite runの三層。
- `analysis`: 差別化の中心は、解決途中で盤面が変わる時系列puzzleと、敵も同じ装置を使うrisk/reward。
- `analysis`: 強みはsynergyが数値だけでなく弾道として見えること。
- `analysis`: 最大riskは予測不能化、draw運と盤面最適解の競合、敵turnの理不尽さ、runの単調化、meta不在。

## 3. ドメイン整理の要約

人手モデルはRun Progression、Combat Encounter、Board Topology、Deck & Build、Effects & Status、Economy & Reward、Meta Progression、Narrative Contentの8領域。Presentation/Inputはadapterとした。

`analysis`: 中心となる不変条件は、決定的ResolutionQueue、一card一zone、支払いと取得の原子性、run終了後の不変性、全状態のtrigger/lifetime、同じseed/inputから同じ結果である。

## 4. Anatomia・コード配線の要約

- `code`: 120 files、424 functions、347 resolved calls、764 unresolved、12 cycle groups、129 static fan-in 0。
- `analysis`: design strength 59.4。ただし421要素が汎用`state-machine`へ集中し、domain coverage 99.3/cohesion 98.6は過大評価しやすい。
- `code`: `AttackMagic.Attack`と`CardMovement.OnEndDrag`はcyclomatic 9。`TileSlot.DecreaseTimes`はfan-out 23。
- `code`: `AttackMagic ↔ TileSlot ↔ Effect`に7関数の中核cycle。
- `analysis`: God Class候補は`UIManager_Battle/Boss`各54.2、`GameManager`50.7。
- `analysis`: ルールはdata objectよりManager/UIへ置かれ、意味上のdomain anemiaがある。

## 5. 仕様不足・矛盾の要約

- P0: 解決順、card zone、turn/cost、buff契約、run lifecycle、勝敗、敵情報、TGS scope。
- 矛盾: 筋力式、buff消費契機、counter/反射、technicalの意味、cost minus、霊陣、拠点ガチャ、無限性。
- `analysis`: content追加より、解決順・card zone・run lifecycleの承認が先。

## 6. Mechanics・内部経済の要約

- `analysis`: 最小動詞は読む、置く、確定する。
- `analysis`: 耐久は発火回数、盤面寿命、破壊時価値の三役を接続する最も強い資源。
- `analysis`: card経済はreward追加のsource優位でdeck肥大とsnowball risk。削除、強化、skipが必要。
- `analysis`: 暫定循環清浄度43/100。本質的complexity 67、偶発的complexity 78。数値balanceは未評価。
- `hypothesis`: 本質的complexityを保ち、偶発的complexityを35以下へ落とすと奥深さに変わる。

## 7. AI Format統合レビューの要約

- `code`: 総合B、Critical 0、High 9、release hold。
- `code`: 回復不能、exhaust card再投入、空山札indexの3 core bug。
- `code`: core自動testなし、run resetなし、不正Event ID softlock、clean build/vendor/license/platform gateなし。
- `analysis`: 最初のrelease gateは3 bug + card保存則 + run reset + deterministic resolution。

## 8. UX提案の要約

- `analysis`: UX目標は「自分の一手でchainを作った」と因果を所有できること。
- P0: 自/敵弾の色・線種分離、敵意図、発火番号、耐久消滅preview、因果log。
- `hypothesis`: tutorialを方向→cost→耐久→敵共有→分岐→buildの7段階で公開する。
- `hypothesis`: 結果damageだけでなくevent列を見せると、初心者支援と上級最適化を両立する。

## 9. Vitia市場仮説の要約

- `analysis`: Vitia scoreはLuxuria 0.768がprimary。Gula 0.597はsecondary不採用。
- `analysis`: 売るべきepisodeは「置く→連鎖→砕ける→敵弾が逆利用」。`無限`は未証明なので使わない。
- `hypothesis`: Propositionは「その一手は、最強の連鎖にも、敵の罠にもなる。5×5の魔法陣ローグライト。」
- `hypothesis`: 実gameplay 15秒 + 3分試遊でgeneric genre説明よりqualified demo start/wishlistが上がる。

## Tensions

### T1 深さ vs 可読性

- Thesis: 共有盤面と耐久消滅は、空間と時間を同時に読む独自の深さを作る。
- Antithesis: 分岐、複数弾、状態、敵turnが重なると、previewが答えを表示するか、無ければ運に見える。
- Evidence needed: 予測不一致率、発火列理解、3連鎖到達時間、preview有無の比較。

### T2 deckbuilding vs board puzzle

- Thesis: 引いたcardから毎turn別の経路を作る即興性がreplay価値を作る。
- Antithesis: 強いboard最適解がcard差を消すか、draw luckがboard思考を上書きする。
- Evidence needed: card選択多様性、tag集中度、drawによる勝敗寄与、同盤面の解法数。

### T3 spectacle vs agency

- Thesis: 多段連鎖の派手な自動解決はTGS/動画で強い。
- Antithesis: 長い自動演出はplayer agencyを奪い、「自分が作った」感覚を弱める。
- Evidence needed: 入力不能時間比、speed利用、chain所有感、映像理解。

### T4 differentiation vs scope

- Thesis: puzzle + deckbuilder + roguelite + story + metaは市場上の厚みになる。
- Antithesis: 8 domainと3 platformを同時に完成させると、最も売れる共有盤面の質が薄まる。
- Evidence needed: TGS vertical sliceの成功KPI、残日数、team throughput、feature cut候補。

### T5 content量 vs rule engine

- Thesis: 敵、card、event、bossが多いほどrun varietyが上がる。
- Antithesis: 今contentを増やすと、未定義ruleとManager cycleが指数的にtest matrixを増やす。
- Evidence needed: 新card一枚当たりのregression工数、rule coverage、content使用率。

### T6 marketable moment vs durable product

- Thesis: 15秒のchain clipで試遊獲得は可能。
- Antithesis: clipが売れても、meta progression、economy、run varietyが弱ければretentionとreviewは続かない。
- Evidence needed: trailer→demo、demo→wishlist、first run→second run、build variety、fatigue。

## Positions for debate

1. **Core-first:** TGS版はPC、短いrun、共有盤面に絞り、meta/story/mobileを後ろへ送る。
2. **Content-first:** 多様なcard/enemy/bossを優先し、見栄えとreplayを先に作る。
3. **UX-first:** resolverは現状を保ち、preview/tutorial/演出で理解可能にする。
4. **Architecture-first:** contentを止め、ResolutionQueue/RunDeckState/RunStateとtestを先に作る。
5. **Market-first:** 15秒proofを先に作り、反応が弱ければ大規模改修前にpositioningを変える。

## Decision criteria

- Unique value: 他作品で代替しにくいか。
- Player agency: playerが結果を自分の判断として説明できるか。
- Learnability: TGSの短時間で最初の価値へ届くか。
- Reliability: core loopが停止せず決定的か。
- Scope efficiency: 一つの改善がbug、UX、content速度、marketing proofを同時に改善するか。
- Evidence: 実測可能で、反証条件を持つか。

## Initial synthesis hypothesis

`hypothesis`: 最も効率がよい順は、(1) 3 core bugとcard保存則、(2) deterministic ResolutionQueueの最小版、(3) 発火列preview/tutorial、(4) 15秒proofのplaytest、(5) 反応を見てcontent、(6) meta/mobileである。Architecture-first単独では遅く、UX-first単独では暗黙ruleを固定できないため、**resolverの薄い正本化とpreviewを同じvertical sliceで行う**。

## Requested conclusion format

Discutereは次を分けて結論する。

1. 面白さの核と、それが壊れる条件。
2. 奥深さとして残すcomplexity、削る偶発的complexity。
3. 売れる可能性を支持/反証する証拠と、誇張してはいけないclaim。
4. TGSまでのP0/P1/P2改善順。
5. 最初の2週間で作る検証可能なvertical slice。
6. 未解決の反対意見と、次に取るdata。
