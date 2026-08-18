using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Datas/Stage")]
public class StageData : ScriptableObject
{
    [SerializeField, Tooltip("ステージID")] private int _stageID;
    [SerializeField, Tooltip("ステージ背景")] private Sprite _background;
    [SerializeField, Tooltip("台座のイメージ")] private Sprite[] _slotSprites;
    [SerializeField, Tooltip("横幅")] private int _width;
    [SerializeField, Tooltip("縦幅")] private int _height;
    [SerializeField, Tooltip("お邪魔魔法陣")] private int _obstacleID;
    [SerializeField, Tooltip("お邪魔マス")] private Vector2Int[] _obstaclePos;
    [SerializeField] private Enemies[] _enemies;
    [SerializeField] private bool _isBossStage = false;

    public int StageID => _stageID;
    public Sprite Background => _background;
    public Sprite[] SlotSprites => _slotSprites;
    public int Width => _width;
    public int Height => _height;
    public int ObstacleID => _obstacleID;
    public Vector2Int[] ObstaclePos => _obstaclePos;
    public Enemies[] Enemies => _enemies;
    public bool IsBossStage => _isBossStage;
}

[Serializable]
public class Enemies
{
    [SerializeField, Tooltip("エネミーID")] private int _enemyID;
    [SerializeField] private int _enemyPosition;

    public int EnemyID => _enemyID;
    public int EnemyPosition => _enemyPosition;
}