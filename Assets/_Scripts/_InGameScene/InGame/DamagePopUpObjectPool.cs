using UnityEngine;
using UnityEngine.Pool;

public class DamagePopUpObjectPool : MonoBehaviour
{
    public static DamagePopUpObjectPool Instance { get; private set; }

    [SerializeField] private DamagePopup _popupPrefab;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private int _defaultCapacity = 20;
    [SerializeField] private int _maxSize = 50;

    private ObjectPool<DamagePopup> _pool;

    private void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<DamagePopup>(
            createFunc: () => Instantiate(_popupPrefab, _canvasTransform),
            actionOnGet: p => p.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: Destroy,
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    public DamagePopup Get(Vector2 anchoredPos, int damage)
    {
        DamagePopup popup = _pool.Get();
        popup.Setup(damage, this);
        popup.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        return popup;
    }

    public void Return(DamagePopup popup) => _pool.Release(popup);
}
