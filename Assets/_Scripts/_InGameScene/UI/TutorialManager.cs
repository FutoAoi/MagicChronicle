using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("-----éQè∆-----")]
    [SerializeField] private GameObject _parent;
    [SerializeField] private TutorialPanel[] _panels;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _beforeButton;

    private int _currentIndex = 0;

    public void Initialize()
    {
        bool isTutorial = GameManager.Instance.StageManager.Stage.StageID == 0;
        for (int i = 0; i < _panels.Length; i++)
        {
            _panels[i].gameObject.SetActive(isTutorial && i == 0);
        }
        _parent.SetActive(isTutorial);

        if (isTutorial)
        {
            _nextButton.onClick.AddListener(() => TurnOver());
            _beforeButton.onClick.AddListener(() => TurnOver(false));
        }
    }

    public void TurnOver(bool isNext = true)
    {
        if (isNext)
        {
            if(_currentIndex + 1 >= _panels.Length)
            {
                _panels[_currentIndex].Hide(true,() =>
                {
                    _parent.SetActive(false);
                });
                return;
            }

            _panels[_currentIndex].Hide();
            _currentIndex++;
            _panels[_currentIndex].Show();
        }
        else
        {
            if (_currentIndex - 1 < 0) return;

            _panels[_currentIndex].Hide(false);
            _currentIndex--;
            _panels[_currentIndex].Show(false);
        }
    }
}
