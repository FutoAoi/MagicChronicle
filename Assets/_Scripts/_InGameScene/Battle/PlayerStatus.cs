using System.Collections.Generic;

public class PlayerStatus
{
    public PlayerType PlayerNowType {  get; private set; }
    public int PlayerMaxHp { get; private set; }
    public int PlayerCurrentHp { get; private set; }
    public int PlayerMaxCost { get; private set; }
    public List<BuffType> DefaultBuffs { get; private set; } = new List<BuffType>();

    public PlayerStatus(PlayerType playerType, int maxHp, int maxCost)
    {
        PlayerNowType = playerType;
        PlayerMaxHp = maxHp;
        PlayerCurrentHp = PlayerMaxHp;
        PlayerMaxCost = maxCost;

        switch (PlayerNowType)
        {
            case PlayerType.Combo:
                DefaultBuffs.Add(BuffType.Combo);
                break;
            case PlayerType.Berserker:
                DefaultBuffs.Add(BuffType.Berserker);
                break;
            case PlayerType.Technical:
                DefaultBuffs.Add(BuffType.Technical);
                break;
        }
    }

    public void SetCurrentHp(int value) => PlayerCurrentHp = value;
    public void AddMaxHp(int value) => PlayerMaxHp += value;
    public void AddMaxCost(int value) => PlayerMaxCost += value;
    public void AddDefaultBuff(BuffType addBuffType) => DefaultBuffs.Add(addBuffType);
}
