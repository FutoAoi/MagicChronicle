using UnityEngine;
using UnityEngine.UI;

public class CardViewByPlayerType : MonoBehaviour
{
    [SerializeField] private Image _cardView;
    [SerializeField] private Image _cardMaskView;
    [SerializeField] private Image _cardBackgroundView;
    
    private void Start()
    {
        PlayerData playerData = GameManager.Instance.PlayerDataBase.GetPlayerData(GameManager.Instance.PlayerType);
        _cardView.sprite = playerData.CardImage;
        _cardMaskView.sprite = playerData.CardMaskImage;
        _cardBackgroundView.sprite = playerData.CardBackgroundImage;
    }
}
