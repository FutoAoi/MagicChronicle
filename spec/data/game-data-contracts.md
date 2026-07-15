# Game data contracts

## Scope

MagicChronicleのカード、deck、buff、enemy、stage、event、map/room、player定義を扱う。現実装は主にUnity ScriptableObjectとserialized assetである。

## Definition and runtime state

- ScriptableObjectはimmutableな定義として扱い、run中に直接変更しない。
- runtimeのcard、status、enemy、runは一意IDを持つinstanceとして別に生成する。
- 新規runはinitial definitionからruntime stateを構築し、前runのmutable stateを共有しない。

## Identifier rules

- 同じdata type内でIDは一意かつ空でない。
- foreign keyはbuild前に存在を検査する。
- event、card、buff、enemy、stageの欠損参照をerrorとする。
- 削除したIDを再利用する場合はsave migrationまたは明示的な非互換versionが必要。

## Required validation

| Data | Checks |
|---|---|
| Card | ID、cost範囲、耐久、effect、sprite、説明文token、tag |
| Buff | ID、trigger、式、stack上限、lifetime、UI icon |
| Enemy | ID、HP、行動、射出点、reward table、prefab |
| Stage | ID、enemy/board/map参照、scene、boss rule |
| Event | ID、本文、1件以上の有効choice、effect、fallback |
| Map | 到達可能性、boss到達、node接続、room種別、seed決定性 |

## Versioning

save formatは未定。導入時は`schemaVersion`、migration、atomic write、backup/fallbackを必須とする。TGS版で永続saveを持たない場合も、session内run stateのversionとreset契約をテストする。
