# Review-derived test plan

## Unit / domain

- Card zone保存則、空山札、reshuffle、exhaust。
- Projectile resolverの決定性、分岐順、耐久、破壊時effect、無限連鎖上限。
- HP clamp、buff式、stack、lifetime、trigger。
- Wallet/Reward/Shop取引の原子性と二重取得防止。
- Run lifecycleのnew/resume/win/lose/abandon reset。

## EditMode / authoring

- 全ScriptableObject ID一意、foreign key、必須参照、範囲。
- Map到達可能性、boss存在、Event choice/fallback。
- UI serialized listとcost上限の整合。

## PlayMode

- Title→Camp→Map→Battle→Reward→Mapのsmoke。
- Battle/Bossで同じcard操作が同じ結果になる。
- 不正Event IDでも操作可能なfallbackから復帰する。
- Pointerなし、touch、drag cancel、scene change中のinput。

## Platform / release

- Clean clone compile、vendor asset検証、license inventory。
- PCをTGS gateとして先行し、iOS/Androidはsafe area、touch、性能を別gateで実施。
- Frame time、memory、連鎖演出時間、長runでのresource leakを計測。

## UX / playtest

- preview有無で予測不一致率、time-to-first-chain、敵damage理解を比較。
- 「自分がchainを作った」同意、demo離脱、再挑戦意図を測る。
- Marketing treatmentはqualified demo startとtrust/誤認を同時に測る。
