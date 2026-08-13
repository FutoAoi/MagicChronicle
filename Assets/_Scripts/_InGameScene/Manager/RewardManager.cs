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
    [SerializeField, Tooltip("スキップボタン")] private Button _skipButton;

    private CardDataBase _cardData;
    private GameManager _gameManager;

    /// <summary>
    /// リワード表示
    /// </summary>
    public void Reward()
    {
        _gameManager = GameManager.Instance;
        _cardData = _gameManager.CardDataBase;
        _skipButton.onClick.AddListener(RewardSkip);
        CriAudioManager.Instance.PlaySe("ME_Win");
        foreach (var card in _rewardCard)
        {
            card.SetCard(_cardData.GetRandomCardIDByRarity(_rarity, _gameManager.GetCardTypeByPlayerType(_gameManager.PlayerType)));
        }
    }

    /// <summary>
    /// 報酬スキップ
    /// </summary>
    public void RewardSkip()
    {
        _gameManager.SceneChange(SceneType.StageSerectScene);
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
