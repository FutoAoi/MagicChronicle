using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Room : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private RoomType _roomType;
    [SerializeField] private int _stageID;
    [SerializeField] private Room[] _nextroom;
    [SerializeField] private Image _roomImage;

    [Header("-----選択可能演出-----")]
    [SerializeField] private float _pulseScale = 1.15f;
    [SerializeField] private float _pulseDuration = 0.6f;

    private MapManager _mapManager;
    private int _floorIndex;
    private int _roomIndex;
    private Vector3 _defaultScale;
    private Tween _pulseTween;
    private bool _isSelectable;

    public Room[] NextRooms => _nextroom;

    private void Awake()
    {
        _defaultScale = transform.localScale;
    }

    /// <summary>
    /// ルーム情報セット
    /// </summary>
    public void SetRoomData(GenerateRoomData roomData, MapManager mapManager)
    {
        _roomType = roomData.RoomType;
        _stageID = roomData.StageID;
        _floorIndex = roomData.FloorIndex;
        _roomIndex = roomData.RoomIndex;
        _mapManager = mapManager;
        _roomImage.sprite = _mapManager.RoomIconData.GetSprite(_roomType);
    }

    public void SetNextRoom(Room[] nextRoom)
    {
        _nextroom = nextRoom;
    }

    /// <summary>
    /// 選択可能かどうかを設定し、見た目(拡大縮小アニメーション)にも反映する
    /// </summary>
    public void SetSelectable(bool selectable)
    {
        _isSelectable = selectable;
        _pulseTween?.Kill();

        if (selectable)
        {
            _pulseTween = transform.DOScale(_defaultScale * _pulseScale, _pulseDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            transform.localScale = _defaultScale;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_isSelectable)
        {
            Debug.Log("今は選択できない部屋");
            return;
        }

        if (_roomType == RoomType.Event)
        {
            FadeManager.Instance.FadePanel(false, () =>
            {
                _mapManager.OpenEventPanel(_roomIndex, _stageID);
                FadeManager.Instance.FadePanel(true, () =>
                {
                    _mapManager.EventTextAnimation();
                });
            });
        }
        else if (_roomType == RoomType.Shop)
        {
            FadeManager.Instance.FadePanel(false, () =>
            {
                _mapManager.OpenShopPanel(_roomIndex);
                FadeManager.Instance.FadePanel(true);
            });
        }
        else if (_roomType == RoomType.Boss)
        {
            GameManager.Instance.StageID = _stageID;
            _mapManager.MoveTo(_roomIndex, true);
        }
        else if (_roomType == RoomType.Rest)
        {
            FadeManager.Instance.FadePanel(false, () =>
            {
                _mapManager.OpenEnhancePanel(_roomIndex);
                FadeManager.Instance.FadePanel(true,() =>
                {
                    _mapManager.EnhanceTextAnimation();
                });
            });
        }
        else
        {
            GameManager.Instance.StageID = _stageID;
            _mapManager.MoveTo(_roomIndex);
        }
    }

    private void OnDisable()
    {
        _pulseTween?.Kill();
    }
}