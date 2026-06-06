using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PhaseChangeButton : MonoBehaviour
{
    [Header("コンポーネント設定")]
    [SerializeField, Tooltip("メインキャンバス")] private Canvas _canvas;
    [SerializeField] private CutInPanel _cutIn;

    [Header("数値設定")]
    [SerializeField, Tooltip("カットインアニメーション時間")] private float _duration = 3f;
    private Button _button;
    private GameManager _gamemanager;
    private GameObject _animPanel;


    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PushButton);
        _gamemanager = GameManager.Instance;
    }
    private void PushButton()
    {
        if (_gamemanager.CurrentPhase != BattlePhase.Set) return;

        _gamemanager.CurrentPhase = BattlePhase.Direction;
        _cutIn.gameObject.SetActive(true);
        _cutIn.CutInAnimation(_duration);
    }
}
