using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private List<EffectData> _effectList;
    private Dictionary<ParticleType, EffectData> _dictionary = new();
    private GameManager _gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameManager.Instance;
        _gameManager.EffectManager = this;

        foreach(EffectData effectData in _effectList)
        {
            _dictionary.Add(effectData.ParticleType, effectData);
        }
    }

    public void ApplyEffect(ParticleType type,Transform parent,RectTransform pos = null)
    {
        EffectData effectData = _dictionary[type];
        GameObject effect = Instantiate(effectData.ParticlePrefab, pos.position,Quaternion.identity);
        effect.transform.SetParent(parent,false);

        if(pos != null && effect.TryGetComponent<RectTransform>(out var rt))
        {
            rt.position = pos.position;
        }
    }
}

[Serializable]
public class EffectData
{
    public ParticleType ParticleType => _particleType;
    public GameObject ParticlePrefab => _particlePrefab;

    [SerializeField] private ParticleType _particleType;
    [SerializeField] private GameObject _particlePrefab;
}
