using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioDatabase")]
public class AudioDataBase : ScriptableObject
{
    public List<AudioData> Entries => _entries;
    [SerializeField] private List<AudioData> _entries;

    private Dictionary<string, AudioData> _cache;

    private void OnEnable()
        => _cache = _entries.ToDictionary(e => e.ID);

    /// <summary>
    /// オーディオデータの取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public AudioData Get(string id)
    {
        if (_cache.TryGetValue(id, out var data)) return data;
        Debug.LogWarning($"AudioData not found: {id}");
        return null;
    }
}