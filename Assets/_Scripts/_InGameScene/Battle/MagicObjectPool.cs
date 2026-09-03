using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MagicObjectPool : MonoBehaviour
{
    private static MagicObjectPool _instance;
    public static MagicObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<MagicObjectPool>();
            }

            return _instance;
        }
    }

    [SerializeField,Tooltip("オブジェクトプール化するオブジェクト")] private AttackMagic _attackMagicPrefab;
    [SerializeField,Tooltip("親オブジェクト")] private RectTransform _rectTransform;

    private Dictionary<AttackMagic, ObjectPool<AttackMagic>> _pools = new();
    private List<AttackMagic> _activeMagics = new List<AttackMagic>();
    public IReadOnlyList<AttackMagic> ActiveMagics => _activeMagics;
    void Start()
    {

    }
    /// <summary>
    /// 魔法を取り出す
    /// </summary>
    /// <returns></returns>
    public AttackMagic GetAttackMagic(AttackMagic prefabOverride = null)
    {
        AttackMagic prefab = prefabOverride != null ? prefabOverride : _attackMagicPrefab;
        return GetPool(prefab).Get();
    }

    private ObjectPool<AttackMagic> GetPool(AttackMagic prefab)
    {
        if (!_pools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<AttackMagic>(
                createFunc: () => Instantiate(prefab, _rectTransform),
                actionOnGet: (obj) => OnGetObject(obj, prefab),
                actionOnRelease: (obj) => OnReleaseObject(obj),
                actionOnDestroy: (obj) => Destroy(obj.gameObject),
                collectionCheck: true,
                defaultCapacity: 3,
                maxSize: 10
            );
            _pools[prefab] = pool;
        }
        return pool;
    }

    private void OnGetObject(AttackMagic attackMagic, AttackMagic prefabKey)
    {
        _activeMagics.Add(attackMagic);
        attackMagic.Initialize(() => _pools[prefabKey].Release(attackMagic));
        attackMagic.gameObject.SetActive(true);
    }
    private void OnReleaseObject(AttackMagic attackMagic)
    {
        _activeMagics.Remove(attackMagic);
    }
}
