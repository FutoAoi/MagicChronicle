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
    private ObjectPool<AttackMagic> _magicPool;
    private List<AttackMagic> _activeMagics = new List<AttackMagic>();
    public IReadOnlyList<AttackMagic> ActiveMagics => _activeMagics;
    void Start()
    {
        _magicPool = new ObjectPool<AttackMagic>(
            createFunc: () => OnCreateObject(),
            actionOnGet: (obj) => OnGetObject(obj),
            actionOnRelease: (obj) => OnReleaseObject(obj),
            actionOnDestroy: (obj) => OnDestroyObject(obj),
            collectionCheck: true,
            defaultCapacity: 3,
            maxSize: 10
        );
    }
    /// <summary>
    /// 魔法を取り出す
    /// </summary>
    /// <returns></returns>
    public AttackMagic GetAttackMagic()
    {
        return _magicPool.Get();
    }
    public void ClearMagic()
    {
        _magicPool.Clear();
    }
    private AttackMagic OnCreateObject()
    {
        return Instantiate(_attackMagicPrefab, _rectTransform);
    }
    private void OnGetObject(AttackMagic attackMagic)
    {
        _activeMagics.Add(attackMagic);
        attackMagic.Initialize(() => _magicPool.Release(attackMagic));
        attackMagic.gameObject.SetActive(true);
    }
    private void OnReleaseObject(AttackMagic attackMagic)
    {
        _activeMagics.Remove(attackMagic);
    }
    private void OnDestroyObject(AttackMagic attackMagic)
    {
        Destroy(attackMagic.gameObject);
    }
}
