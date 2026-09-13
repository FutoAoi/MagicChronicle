using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 複数のパネル(GameObject)を左右の矢印ボタンでOn/Off切り替えする。
/// 演出なし、押した瞬間に即切り替え。
///
/// 使い方:
/// 1. このスクリプトを空のGameObject(例: "PanelSwitcher")にアタッチ
/// 2. Panels に切り替えたいパネルのGameObjectをInspectorから順番に登録
/// 3. PrevButton / NextButton に矢印ボタン(Button)をアサイン
/// </summary>
public class PanelSwitcher : MonoBehaviour
{
    [Tooltip("切り替え対象のパネル。表示したい順番に並べる")]
    [SerializeField] private GameObject[] panels;

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;

    [Tooltip("開始時に表示するパネルのインデックス")]
    [SerializeField] private int startIndex = 0;

    [Header("演出設定")]
    [SerializeField] private float _duration = 0.3f;
    [SerializeField] private float _slideOffsetX = 800f;

    public int CurrentIndex;

    private RectTransform[] _rects;
    private Vector2[] _basePositions;
    private bool _isAnimating = false;

    private void Awake()
    {
        if (prevButton != null) prevButton.onClick.AddListener(ShowPrev);
        if (nextButton != null) nextButton.onClick.AddListener(ShowNext);

        _rects = new RectTransform[panels.Length];
        _basePositions = new Vector2[panels.Length];
        for (int i = 0; i < panels.Length; i++)
        {
            _rects[i] = panels[i].GetComponent<RectTransform>();
            _basePositions[i] = _rects[i].anchoredPosition;
        }
    }

    private void Start()
    {
        CurrentIndex = Mathf.Clamp(startIndex, 0, panels.Length - 1);
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == CurrentIndex);
            _rects[i].anchoredPosition = _basePositions[i];
        }
    }

    public void ShowNext()
    {
        if (panels.Length == 0 || _isAnimating) return;
        int nextIndex = (CurrentIndex + 1) % panels.Length;
        SwitchPanel(nextIndex, true);
    }

    public void ShowPrev()
    {
        if (panels.Length == 0 || _isAnimating) return;
        int prevIndex = (CurrentIndex - 1 + panels.Length) % panels.Length;
        SwitchPanel(prevIndex, false);
    }

    private void SwitchPanel(int nextIndex, bool isNext)
    {
        int currentIndex = CurrentIndex;
        _isAnimating = true;
        SetButtonsInteractable(false);

        RectTransform currentRt = _rects[currentIndex];
        RectTransform nextRt = _rects[nextIndex];

        float exitOffset = isNext ? -_slideOffsetX : _slideOffsetX;
        float enterOffset = isNext ? _slideOffsetX : -_slideOffsetX;

        panels[nextIndex].SetActive(true);
        nextRt.anchoredPosition = _basePositions[nextIndex] + new Vector2(enterOffset, 0f);

        Sequence seq = DOTween.Sequence();
        seq.Join(currentRt.DOAnchorPos(_basePositions[currentIndex] + new Vector2(exitOffset, 0f), _duration)
            .SetEase(Ease.OutCubic));
        seq.Join(nextRt.DOAnchorPos(_basePositions[nextIndex], _duration).SetEase(Ease.OutCubic));
        seq.OnComplete(() =>
        {
            panels[currentIndex].SetActive(false);
            currentRt.anchoredPosition = _basePositions[currentIndex];
            CurrentIndex = nextIndex;
            _isAnimating = false;
            SetButtonsInteractable(true);
        });
    }

    private void SetButtonsInteractable(bool value)
    {
        if (prevButton != null) prevButton.interactable = value;
        if (nextButton != null) nextButton.interactable = value;
    }
}