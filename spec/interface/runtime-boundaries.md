# Runtime boundaries

## External surface

現観測ではonline API、multiplayer、server authorityはない。主な外部境界はUnity runtime、Input System、scene/prefab serialization、CRIWARE audio、platform build environmentである。

## Boundary contracts

- Input adapterはpointer/touch/keyboard/gamepadをdomain commandへ変換し、`Mouse.current`の存在を前提にしない。
- Presentationはdomain eventを表示する。UI、animation、soundからrule stateを直接変更しない。
- CRIWAREのcue sheet、version、checksum、license、配置をsetup正本へ固定する。
- Scene changeは`StartNewRun / ResumeRun / EndRun`のapplication serviceを経由する。
- ScriptableObject lookup失敗はexception/blank panelではなく、検証時errorとruntimeの安全なfallbackを持つ。

## Non-goals for the current slice

- Online account、ranking、cloud save、payment、remote configuration。
- これらを追加する場合はauthentication、privacy、failure/retry、offline behaviorを別interface仕様として追加する。
