/// <summary>
/// 戦闘時のフェイズ
/// </summary>
public enum BattlePhase: byte
{
    Draw,
    Set,
    Action,
    Direction,
    End,
    Reward,
    BuildStage
}
/// <summary>
/// 攻撃の方向指定
/// </summary>
public enum MagicVector : byte
{
    UP,
    Down,
    Right,
    Left,
}
/// <summary>
/// カードのレアリティ
/// </summary>
public enum CardRarity : byte
{
    Uncommon,
    Common,
    Rare,
    Epic,
    Legendary
}
/// <summary>
/// エネミーの攻撃種類
/// </summary>
public enum EnemyAttackType : byte
{
    Buff,
    Normal,
    BoardInterface,
}
/// <summary>
/// シーンの名前
/// </summary>
public enum SceneType : byte
{
    TitleScene,
    InGameScene,
    StageSerectScene,
    CampScene
}
/// <summary>
/// カードのタイプ
/// </summary>
public enum CardType : byte
{
    Nomal,
    Combo,
    Berserker,
    Variation
}
/// <summary>
/// カードがどこにあるか
/// </summary>
 public enum InGameDeckType : byte
{
    Deck,
    Discard,
    Remove,
    Hand
}

public enum PlayerType
{
    Combo,
    Berserker,
    Technical
}

public enum BuffType : byte
{
    Combo,
    Berserker,
    Technical,
    Poizon,
    Strength,
    Weaken,
    Tired,
    Regeneration,
    Counter,
    CostMinus,
    CostPlus,
    CardPlus,
    Rapid,
    End
}