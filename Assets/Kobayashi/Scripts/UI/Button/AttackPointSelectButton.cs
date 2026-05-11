using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class AttackPointSelectButton : MonoBehaviour
{
    [Header("-----参照-----")]
    [SerializeField] private Sprite _dark;
    [SerializeField] private Sprite _light;

    [Header("-----数値設定-----")]
    [SerializeField, Tooltip("非選択時透明度")] private float _alpha = 0.7f;
    [SerializeField, Tooltip("演出時間")] private float _duration = 0.4f;
    [SerializeField, Tooltip("選択時倍率")] private float _selectMag = 1.3f;

    public int AttackNumber;
    public bool IsSelect = false;

    AttackPointManager _attackPointManager;
    Image _img;
    GameManager _gameManager;
    RectTransform _rt;
    Button _button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!IsSelect)
        ResetButton();
    }

    private void ResetButton()
    {
        _gameManager = GameManager.Instance;
        _img = GetComponent<Image>();
        _attackPointManager = GetComponentInParent<AttackPointManager>();
        _rt = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(RegisterAttackPosition);
        IsSelect = false;
        _img.sprite = _dark;
        _img.color = new Color(1f, 1f, 1f, _alpha);
    }

    /// <summary>
    /// 攻撃場所変更
    /// </summary>
    public void RegisterAttackPosition()
    {
        //攻撃場所変更処理
        if (_gameManager == null)
        {
            ResetButton();
        }
        _gameManager.AttackManager.AttackStartPos = AttackNumber;
        IsSelect = true;
        _attackPointManager.ChangeButtonState(AttackNumber);

        //演出
        _rt.DOScale(Vector2.one * _selectMag,_duration * 0.5f)
            .SetEase(Ease.OutSine);
        _img.sprite = _light;
        _img.DOColor(Color.white, _duration * 0.5f);
    }

    /// <summary>
    /// 非選択演出
    /// </summary>
    public void CancelRegister()
    {
        IsSelect = false;

        _rt.DOScale(Vector2.one, _duration)
            .SetEase(Ease.OutSine);
        _img.sprite = _dark;
        _img.DOColor(new Color(1f,1f,1f,_alpha),_duration);
    }
}
