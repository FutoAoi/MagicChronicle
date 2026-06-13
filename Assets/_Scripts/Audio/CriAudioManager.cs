using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CriWare;

/// <summary>
/// CRIミドルウェアを使用したオーディオ管理クラス（2Dゲーム用）
/// シングルトンパターンで実装。BGM・SE の再生/停止/音量/フェードを一元管理します。
/// </summary>
public class CriAudioManager : MonoBehaviour
{
    // ─── シングルトン ───────────────────────────────────────────────
    public static CriAudioManager Instance { get; private set; }

    // ─── インスペクター設定 ─────────────────────────────────────────
    [Header("ACF設定")]
    [Tooltip("StreamingAssets内のACFファイル名（拡張子なし）")]
    [SerializeField] private string acfFileName = "GameAudioSetting";

    [Header("ACBファイル名（StreamingAssets内、拡張子なし）")]
    [SerializeField] private string bgmAcbName = "BGM";
    [SerializeField] private string seAcbName = "SE";

    [Header("初期音量 (0.0 〜 1.0)")]
    [Range(0f, 1f)][SerializeField] private float initialMasterVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float initialBgmVolume = 1.0f;
    [Range(0f, 1f)][SerializeField] private float initialSeVolume = 1.0f;

    // ─── 内部状態 ───────────────────────────────────────────────────
    private CriAtomExPlayer _bgmPlayer;
    private CriAtomExPlayer _sePlayer;
    private CriAtomExAcb _bgmAcb;
    private CriAtomExAcb _seAcb;
    private CriAtomExPlayback _bgmPlayback;   // Update()に渡すPlayback

    private float _masterVolume;
    private float _bgmVolume;
    private float _seVolume;

    private string _currentBgmCueName = string.Empty;
    private Coroutine _fadeCoroutine;

    // ─── 音量プロパティ ─────────────────────────────────────────────
    public float MasterVolume
    {
        get => _masterVolume;
        set { _masterVolume = Mathf.Clamp01(value); ApplyBgmVolume(); }
    }

    public float BgmVolume
    {
        get => _bgmVolume;
        set { _bgmVolume = Mathf.Clamp01(value); ApplyBgmVolume(); }
    }

    public float SeVolume
    {
        get => _seVolume;
        set { _seVolume = Mathf.Clamp01(value); }
    }

    // ─── ライフサイクル ─────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeCri();
    }

    private void OnDestroy()
    {
        ReleaseCri();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                               UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // CriAtomが新シーンで再生成されるのを1フレーム待ってからACBを再取得
        StartCoroutine(ReloadAcbAfterSceneLoad());
    }

    private IEnumerator ReloadAcbAfterSceneLoad()
    {
        yield return null; // CriAtomの初期化を待つ

        // すでにCueSheetが登録済みなら再登録しない
        if (CriAtom.GetAcb("BGM") == null)
        {
            CriAtom.AddCueSheet("BGM", "BGM.acb", "BGM.awb", null);
        }
        if (CriAtom.GetAcb("SE") == null)
        {
            CriAtom.AddCueSheet("SE", "SE.acb", null, null);
        }

        _bgmAcb = CriAtom.GetAcb("BGM");
        _seAcb = CriAtom.GetAcb("SE");

        Debug.Log($"[CriAudioManager] シーン再ロード後 BGM={_bgmAcb != null} SE={_seAcb != null}");

        // シーン遷移前に再生していたBGMを復元
        if (!string.IsNullOrEmpty(_currentBgmCueName))
        {
            PlayBgm(_currentBgmCueName);
        }
    }

    // ─── 初期化 / 解放 ──────────────────────────────────────────────
    private void InitializeCri()
    {
        // ACF 登録
        string acfPath = System.IO.Path.Combine(
            Application.streamingAssetsPath, acfFileName + ".acf");
        CriAtomEx.RegisterAcf(null, acfPath);

        // ACB 読み込み
        string bgmAcbPath = System.IO.Path.Combine(Application.streamingAssetsPath, bgmAcbName + ".acb");
        string seAcbPath = System.IO.Path.Combine(Application.streamingAssetsPath, seAcbName + ".acb");

        Debug.Log($"[CriAudioManager] BGM ACBパス: {bgmAcbPath}");
        Debug.Log($"[CriAudioManager] SE  ACBパス: {seAcbPath}");
        Debug.Log($"[CriAudioManager] BGM ファイル存在: {System.IO.File.Exists(bgmAcbPath)}");
        Debug.Log($"[CriAudioManager] SE  ファイル存在: {System.IO.File.Exists(seAcbPath)}");

        CriAtom.AddCueSheet("BGM", "BGM.acb", "BGM.awb", null);
        _bgmAcb = CriAtom.GetAcb("BGM");
        CriAtom.AddCueSheet("SE", "SE.acb", null, null);
        _seAcb = CriAtom.GetAcb("SE");

        Debug.Log($"[CriAudioManager] BGM ACBロード結果: {(_bgmAcb != null ? "成功" : "失敗")}");
        Debug.Log($"[CriAudioManager] SE  ACBロード結果: {(_seAcb != null ? "成功" : "失敗")}");

        // プレーヤー生成（2D設定を明示）
        _bgmPlayer = new CriAtomExPlayer();
        _bgmPlayer.SetPanType(CriAtomEx.PanType.Pan3d);
        _bgmPlayer.Set3dSource(null);
        _bgmPlayer.SetVolume(1.0f);

        _sePlayer = new CriAtomExPlayer();
        _sePlayer.SetPanType(CriAtomEx.PanType.Pan3d);
        _sePlayer.Set3dSource(null);
        _sePlayer.SetVolume(1.0f);

        // 初期音量設定
        _masterVolume = initialMasterVolume;
        _bgmVolume = initialBgmVolume;
        _seVolume = initialSeVolume;

        // ACB内のキュー一覧をログ出力（デバッグ用）
        PrintCueList(_bgmAcb, "BGM");
        PrintCueList(_seAcb, "SE");

        Debug.Log("[CriAudioManager] 初期化完了");
    }

    private void ReleaseCri()
    {
        StopFade();

        _bgmPlayer?.Dispose();
        _sePlayer?.Dispose();
        _bgmAcb?.Dispose();
        _seAcb?.Dispose();

        CriAtomEx.UnregisterAcf();

        Debug.Log("[CriAudioManager] リソース解放完了");
    }

    // ─── BGM ────────────────────────────────────────────────────────

    /// <summary>BGMを再生します（デフォルトでループ有効）。</summary>
    public void PlayBgm(string cueName, bool loop = true)
    {
        if (_bgmAcb == null)
        {
            Debug.LogWarning("[CriAudioManager] BGM ACBが未ロードです。");
            return;
        }

        StopFade();
        _currentBgmCueName = cueName;

        _bgmPlayer.SetCue(_bgmAcb, cueName);
        _bgmPlayer.Loop(loop);
        _bgmPlayer.SetVolume(_bgmVolume * _masterVolume);
        _bgmPlayback = _bgmPlayer.Start();  // Playbackを保持

        Debug.Log($"[CriAudioManager] BGM再生: {cueName}  loop={loop}");
        Debug.Log($"[CriAudioManager] Playback ID: {_bgmPlayback.id}");
        StartCoroutine(CheckPlaybackStatus());
    }

    /// <summary>BGMを即時停止します。</summary>
    public void StopBgm(bool withRelease = false)
    {
        StopFade();
        if (withRelease)
            _bgmPlayer.Stop();
        else
            _bgmPlayer.StopWithoutReleaseTime();

        _currentBgmCueName = string.Empty;
        Debug.Log("[CriAudioManager] BGM停止");
    }

    /// <summary>BGMを一時停止します。</summary>
    public void PauseBgm()
    {
        _bgmPlayer.Pause();
        Debug.Log("[CriAudioManager] BGM一時停止");
    }

    /// <summary>BGMを再開します。</summary>
    public void ResumeBgm()
    {
        _bgmPlayer.Resume(CriAtomEx.ResumeMode.AllPlayback);
        Debug.Log("[CriAudioManager] BGM再開");
    }

    // ─── SE ─────────────────────────────────────────────────────────

    /// <summary>SEを再生します。</summary>
    public void PlaySe(string cueName)
    {
        if (_seAcb == null)
        {
            Debug.LogWarning("[CriAudioManager] SE ACBが未ロードです。");
            return;
        }

        _sePlayer.SetCue(_seAcb, cueName);
        _sePlayer.SetVolume(_seVolume * _masterVolume);
        _sePlayer.Start();

        Debug.Log($"[CriAudioManager] SE再生: {cueName}");
    }

    /// <summary>再生中のSEをすべて停止します。</summary>
    public void StopAllSe()
    {
        _sePlayer.StopWithoutReleaseTime();
        Debug.Log("[CriAudioManager] SE全停止");
    }

    // ─── フェード ────────────────────────────────────────────────────

    /// <summary>BGMをフェードインして再生します。</summary>
    public void FadeInBgm(string cueName, float duration = 1.0f, bool loop = true)
    {
        StopFade();
        _currentBgmCueName = cueName;

        _bgmPlayer.SetCue(_bgmAcb, cueName);
        _bgmPlayer.Loop(loop);
        _bgmPlayer.SetVolume(0f);
        _bgmPlayback = _bgmPlayer.Start();

        _fadeCoroutine = StartCoroutine(FadeCoroutine(0f, _bgmVolume * _masterVolume, duration));
    }

    /// <summary>BGMをフェードアウトして停止します。</summary>
    public void FadeOutBgm(float duration = 1.0f)
    {
        StopFade();
        float startVol = _bgmVolume * _masterVolume;
        _fadeCoroutine = StartCoroutine(FadeCoroutine(startVol, 0f, duration, stopOnComplete: true));
    }

    /// <summary>BGMをクロスフェードで切り替えます。</summary>
    public void CrossFadeBgm(string newCueName, float duration = 1.0f, bool loop = true)
    {
        StopFade();
        _fadeCoroutine = StartCoroutine(CrossFadeCoroutine(newCueName, duration, loop));
    }

    // ─── コルーチン ─────────────────────────────────────────────────
    private IEnumerator FadeCoroutine(
        float fromVolume,
        float toVolume,
        float duration,
        bool stopOnComplete = false)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float vol = Mathf.Lerp(fromVolume, toVolume, elapsed / duration);
            _bgmPlayer.SetVolume(vol);
            _bgmPlayer.Update(_bgmPlayback);
            yield return null;
        }
        _bgmPlayer.SetVolume(toVolume);
        _bgmPlayer.Update(_bgmPlayback);

        if (stopOnComplete)
        {
            _bgmPlayer.StopWithoutReleaseTime();
            _currentBgmCueName = string.Empty;
        }
        _fadeCoroutine = null;
    }

    private IEnumerator CrossFadeCoroutine(string newCueName, float duration, bool loop)
    {
        float startVol = _bgmVolume * _masterVolume;
        float half = duration * 0.5f;

        // フェードアウト
        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float vol = Mathf.Lerp(startVol, 0f, elapsed / half);
            _bgmPlayer.SetVolume(vol);
            _bgmPlayer.Update(_bgmPlayback);
            yield return null;
        }

        // 新しいBGMをフェードイン
        _bgmPlayer.StopWithoutReleaseTime();
        _currentBgmCueName = newCueName;
        _bgmPlayer.SetCue(_bgmAcb, newCueName);
        _bgmPlayer.Loop(loop);
        _bgmPlayer.SetVolume(0f);
        _bgmPlayback = _bgmPlayer.Start();

        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            float vol = Mathf.Lerp(0f, startVol, elapsed / half);
            _bgmPlayer.SetVolume(vol);
            _bgmPlayer.Update(_bgmPlayback);
            yield return null;
        }

        _bgmPlayer.SetVolume(startVol);
        _bgmPlayer.Update(_bgmPlayback);
        _fadeCoroutine = null;
    }

    private IEnumerator CheckPlaybackStatus()
    {
        // 数フレーム待ってからステータスを確認
        yield return new WaitForSeconds(0.5f);
        var status = _bgmPlayback.GetStatus();
        Debug.Log($"[CriAudioManager] 0.5秒後のPlaybackステータス: {status}");
        // Playing=再生中, Removed=再生できなかった, Prep=準備中
    }

    private void StopFade()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }

    // ─── デバッグユーティリティ ────────────────────────────────────────
    private void PrintCueList(CriAtomExAcb acb, string label)
    {
        if (acb == null) return;
        CriAtomEx.CueInfo[] cueInfoList = acb.GetCueInfoList();
        if (cueInfoList == null || cueInfoList.Length == 0)
        {
            Debug.LogWarning($"[CriAudioManager] {label} ACB にキューが見つかりません");
            return;
        }
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine($"[CriAudioManager] {label} ACB のキュー一覧 ({cueInfoList.Length}件):");
        foreach (var info in cueInfoList)
            sb.AppendLine($"  ID:{info.id}  名前:{info.name}");
        Debug.Log(sb.ToString());
    }

    // ─── 内部ユーティリティ ─────────────────────────────────────────
    private void ApplyBgmVolume()
    {
        float vol = _bgmVolume * _masterVolume;
        _bgmPlayer.SetVolume(vol);
        _bgmPlayer.Update(_bgmPlayback);
    }
}