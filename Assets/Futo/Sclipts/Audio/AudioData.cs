using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public string ID => _id;
    public float Volume => _volume;
    public AudioMixerGroup MixerGroup => _mixerGroup;

    [Header("基本設定")]
    [SerializeField, Tooltip("取得する際のID")] private string _id;
    [SerializeField, Tooltip("クリップ(複数の場合ランダム)")] private AudioClip[] _clips;
    [SerializeField, Tooltip("音量"),Range(0f, 1f)] private float _volume = 1f;

    [Header("ミキサー")]
    [SerializeField] private AudioMixerGroup _mixerGroup;

    /// <summary>
    /// クリップの取得
    /// </summary>
    /// <returns></returns>
    public AudioClip GetClip()
        => _clips == null || _clips.Length == 0 ? null
         : _clips[Random.Range(0, _clips.Length)];
}