using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private SoundDataBaseSo _soundDataBase;

    [SerializeField]
    private AudioSource _bgmSource;

    [SerializeField]
    private Transform _seRoot;

    [SerializeField]
    private int _sePoolSize = 10;

    private readonly Queue<AudioSource> _seAudioSourcePools = new Queue<AudioSource>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (_seRoot == null)
        {
            _seRoot = transform;
        }

        for (int i = 0; i < _sePoolSize; i++)
        {
            var instance = new GameObject("SeAudioSource_" + i, typeof(AudioSource));
            instance.transform.SetParent(_seRoot);
            instance.gameObject.SetActive(false);
            _seAudioSourcePools.Enqueue(instance.GetComponent<AudioSource>());
        }

    }

    public void PlayBGM(string key)
    {
        StopBGM();
        var soundData = _soundDataBase.GetSoundData(key);

        if (soundData == null)
        {
            Debug.LogWarning("Sound Data not found: " + key);
            return;
        }

        _bgmSource.PrepareAudioSource(soundData);

        _bgmSource.Play();
    }

    public void StopBGM()
    {
        if (_bgmSource.isPlaying)
        {
            _bgmSource.Stop();
        }
    }

    public void PlaySe(string key)
    {
        var soundData = _soundDataBase.GetSoundData(key);
        if (soundData == null)
        {
            Debug.LogWarning("Sound Data not found: " + key);
            return;
        }

        AudioSource seAudioSource = default;
        if (_seAudioSourcePools.TryDequeue(out AudioSource source))
        {
            seAudioSource = source;
        }
        else
        {
            seAudioSource = new GameObject("seAudioSource_" + "NewInstance", typeof(AudioSource)).GetComponent<AudioSource>();
        }

        seAudioSource.PrepareAudioSource(soundData);
        seAudioSource.gameObject.SetActive(true);
        seAudioSource.Play();

        StartCoroutine(ReturnToPoolAfterPlaying(seAudioSource));
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        source.gameObject.SetActive(false);
        _seAudioSourcePools.Enqueue(source);
    }
}
