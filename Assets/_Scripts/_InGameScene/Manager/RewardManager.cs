using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    public RewardCard[] RewardCards => _rewardCard;
    public bool IsFinishTurnAnimation = false;

    [SerializeField, Tooltip("報酬一覧")] private RewardCard[] _rewardCard;
    [SerializeField, Tooltip("抽選されるレアリティ")] private CardRarity _rarity;

    [Header("ボタン設定")]
    [SerializeField, Tooltip("獲得ボタン")] private Button _getButton;
    [SerializeField, Tooltip("スキップボタン")] private Button _skipButton;

    private CardDataBase _cardData;
    private int _serectReward = 0;
    private GameManager _gameManager;

    /// <summary>
    /// リワード表示
    /// </summary>
    public void Reward()
    {
        _gameManager = GameManager.Instance;
        _cardData = _gameManager.CardDataBase;
        _getButton.onClick.AddListener(GetReward);
        _skipButton.onClick.AddListener(RewardSkip);
        foreach (var card in _rewardCard)
        {
            card.SetCard(_cardData.GetRandomCardIDByRarity(_rarity, _gameManager.GetCardTypeByPlayerType(_gameManager.PlayerType)));
        }
    }

    /// <summary>
    /// 報酬取得
    /// </summary>
    public void GetReward()
    {
        DeckManager.Instance.AddDeck(_rewardCard[_serectReward].CardID);
        _gameManager.SceneChange(SceneType.StageSerectScene);
    }

    /// <summary>
    /// 報酬スキップ
    /// </summary>
    public void RewardSkip()
    {
        _gameManager.SceneChange(SceneType.StageSerectScene);
    }

    /// <summary>
    /// 獲得する報酬の数を設定
    /// </summary>
    /// <param name="number"></param>
    public void SetRewardNumber(int number)
    {
        _serectReward = number;
    }

    /// <summary>
    /// カードがめくれるアニメーション
    /// </summary>
    public IEnumerator RewardAnimation()
    {
        foreach(RewardCard rewardCard in _rewardCard)
        {
            rewardCard.TurnCardAnimation();
            yield return new WaitUntil(() => rewardCard.IsFinish);
        }
        IsFinishTurnAnimation = true;
    }
}
