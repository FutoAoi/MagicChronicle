using System.Collections.Generic;

public class PlayerStatus
{
    private PlayerType _playerType;
    private int PlayerMaxHp { get; set; }
    private int CurrentHp { get; set; }
    private int MaxCost { get; set; }
    private List<BuffType> _defaultBuffs = new List<BuffType>();

    PlayerStatus(PlayerType playerType, int maxHp, int maxCost)
    {
        _playerType = playerType;
        PlayerMaxHp = maxHp;
        CurrentHp = PlayerMaxHp;
        MaxCost = maxCost;

        switch (_playerType)
        {
            case PlayerType.Combo:
                _defaultBuffs.Add(BuffType.Combo);
                break;
            case PlayerType.Berserker:
                _defaultBuffs.Add(BuffType.Berserker);
                break;
            case PlayerType.Technical:
                _defaultBuffs.Add(BuffType.Technical);
                break;
        }
    }

    public void SetCurrentHp(int value) => CurrentHp = value;
    public void AddMaxHp(int value) => PlayerMaxHp += value;
    public void AddMaxCost(int value) => MaxCost += value;
    public void AddDefaultBuff(BuffType addBuffType) => _defaultBuffs.Add(addBuffType);
}
