using UnityEngine;
using UnityEngine.UI;

public class CardViewByPlayerType : MonoBehaviour
{
    [SerializeField] private Image _cardView;
    [SerializeField] private Image? _cardMaskView;
    [SerializeField] private Image? _cardBackgroundView;
    [SerializeField] private Image? _cardBackImage;
    
    private void Start()
    {
        PlayerData playerData = GameManager.Instance.PlayerDataBase.GetPlayerData(GameManager.Instance.PlayerType);
        if(_cardView  != null)
        _cardView.sprite = playerData.CardImage;
        if(_cardMaskView != null)
        _cardMaskView.sprite = playerData.CardMaskImage;
        if(_cardBackgroundView != null)
        _cardBackgroundView.sprite = playerData.CardBackgroundImage;
        if(_cardBackImage != null)
        _cardBackImage.sprite = playerData.CardBackImage;
    }
}
