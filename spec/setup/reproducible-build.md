# Reproducible build setup

## Known baseline

- Unity Editor: `6000.3.8f1`（既存AI Formatレビュー観測）。
- Target repository commit: `ed9846e62728080798edf34f70237689ae807428`。
- Package lockはrepositoryに存在する。

## Missing prerequisites

`.gitignore`で除外されたCRIWARE、StreamingAssets、Fonts等について、取得元、version、checksum、配置、licenseが未文書化である。このためclean cloneからのbuildは未確認で、成功を主張しない。

## Required setup contract

1. 権限を持つartifact storeからvendor assetを取得する。
2. versionとSHA-256を検証し、決められたUnity pathへ配置する。
3. Unity 6000.3.8f1でcompileする。
4. Title、Camp、StageSelect、Battle、Boss、Eventのscene smokeを行う。
5. 対象platformを一つずつbuildし、artifact hashとlogを保存する。

秘密情報、署名URL、vendor binary自体を本specへ保存しない。
