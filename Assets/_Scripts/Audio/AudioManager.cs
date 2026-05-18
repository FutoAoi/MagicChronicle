using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Audio;
using System.Collections;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("データベース")]
    [SerializeField, Tooltip("オーディオデータベース")] AudioDataBase _database;

    [Header("AudioMixer")]
    [SerializeField, Tooltip("ミキサー")] AudioMixer _mixer;

    [Header("BGM Sources (クロスフェード用に2つ)")]
    [SerializeField, Tooltip("フェード用A")] AudioSource _bgmSourceA;
    [SerializeField, Tooltip("フェード用B")] AudioSource _bgmSourceB;

    [Header("SE Pool")]
    [SerializeField, Tooltip("SEのプールのプレハブ")] AudioSource _seSourcePrefab;

    private IObjectPool<AudioSource> _sePool;
    private AudioSource _activeBgm;
    private Coroutine _fadeCoroutine;

    private const string KEY_MASTER = "vol_master";
    private const string KEY_BGM = "vol_bgm";
    private const string KEY_SE = "vol_se";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitPool();
        LoadVolumeSettings();
    }

    public void PlaySE(string id)
    {
        var data = _database.Get(id);
        if (data == null) return;

        var src = _sePool.Get();
        src.clip = data.GetClip();
        src.volume = data.Volume;
        src.outputAudioMixerGroup = data.MixerGroup;
        src.spatialBlend = 0f;

        src.Play();
        StartCoroutine(ReleaseWhenDone(src));
    }

    IEnumerator ReleaseWhenDone(AudioSource src)
    {
        yield return new WaitForSeconds(src.clip.length);
        _sePool.Release(src);
    }

    public void PlayBGM(string id, float fadeTime = 1.0f)
    {
        var data = _database.Get(id);
        if (data == null) return;

        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(CrossFadeBGM(data, fadeTime));
    }

    IEnumerator CrossFadeBGM(AudioData data, float dur)
    {
        var next = (_activeBgm == _bgmSourceA) ? _bgmSourceB : _bgmSourceA;
        next.clip = data.GetClip();
        next.volume = 0f;
        next.loop = true;
        next.Play();

        float t = 0f;
        float prevVol = _activeBgm != null ? _activeBgm.volume : 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            float r = t / dur;
            next.volume = r * data.Volume;
            if (_activeBgm != null) _activeBgm.volume = (1f - r) * prevVol;
            yield return null;
        }

        _activeBgm?.Stop();
        _activeBgm = next;
    }

    public void StopBGM(float fadeTime = 0.5f)
        => StartCoroutine(FadeOut(_activeBgm, fadeTime));

    IEnumerator FadeOut(AudioSource src, float dur)
    {
        if (src == null) yield break;
        float startVol = src.volume;
        for (float t = 0f; t < dur; t += Time.deltaTime)
        {
            src.volume = Mathf.Lerp(startVol, 0f, t / dur);
            yield return null;
        }
        src.Stop();
    }

    public void SetVolume(string group, float normalizedVol)
    {
        float db = normalizedVol > 0.0001f
            ? Mathf.Log10(normalizedVol) * 20f
            : -80f;
        _mixer.SetFloat(group, db);
        PlayerPrefs.SetFloat("vol_" + group, normalizedVol);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        SetVolume("Master", PlayerPrefs.GetFloat(KEY_MASTER, 1f));
        SetVolume("BGM", PlayerPrefs.GetFloat(KEY_BGM, 0.8f));
        SetVolume("SE", PlayerPrefs.GetFloat(KEY_SE, 1f));
    }

    void InitPool()
    {
        _sePool = new ObjectPool<AudioSource>(
            createFunc: () => Instantiate(_seSourcePrefab, transform),
            actionOnGet: s => s.gameObject.SetActive(true),
            actionOnRelease: s => s.gameObject.SetActive(false),
            defaultCapacity: 10, maxSize: 30
        );
    }
}