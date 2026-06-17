using UnityEngine;
using UnityEngine.UI;

public class SlotMagicCircleShadow : MonoBehaviour
{
    [Header("参照")]
    [SerializeField, Header("親オブジェクト")] private GameObject _parent;
    [SerializeField, Header("魔法陣")] private Image _circle;
    [SerializeField, Header("上印")] private Image _upArrowImage;
    [SerializeField, Header("右印")] private Image _rightArrowImage;
    [SerializeField, Header("左印")] private Image _leftArrowImage;
    [SerializeField, Header("下印")] private Image _downArrowImage;

    private GameManager _gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    public void DisplayShadow(bool isDisplay = false,int id = 0)
    {
        _parent.SetActive(isDisplay);

        if (isDisplay)
        {
            CardData data = _gameManager.CardDataBase.GetCardData(id);
            _circle.gameObject.SetActive(true);
            _circle.sprite = data.MagicSprite;
            _upArrowImage.gameObject.SetActive(false);
            _downArrowImage.gameObject.SetActive(false);
            _rightArrowImage.gameObject.SetActive(false);
            _leftArrowImage.gameObject.SetActive(false);
            foreach(MagicVector vector in data.DisplayArrowVector)
            {
                GetArrowImage(vector).gameObject.SetActive(true);
            }
        }
    }

    private Image GetArrowImage(MagicVector vector)
    {
        return vector switch
        {
            MagicVector.UP => _upArrowImage,
            MagicVector.Right => _rightArrowImage,
            MagicVector.Left => _leftArrowImage,
            MagicVector.Down => _downArrowImage,
            _ => null
        };
    }
}
