using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public CardDataBase CardDataBase => _cardDataBase;
    public BuffDataBase BuffDataBase => _buffDataBase;
    public StageDataBase StageDataBase => _stageDataBase;
    public EnemyDataBase EnemyDataBase => _enemyDataBase;
    public GenerateMapData GenerateMapData => _generateMapData;
    public PlayerDataBase PlayerDataBase => _playerDataBase;
    public KeywordDataBase KeywordDataBase => _keywordDataBase;
    public PlayerType PlayerType => _playerType;
    public PlayerStatus PlayerStatus => _playerStatus;

    [Header("データベース")]
    [SerializeField, Tooltip("カード")] private CardDataBase _cardDataBase;
    [SerializeField, Tooltip("バフ")] private BuffDataBase _buffDataBase;
    [SerializeField, Tooltip("ステージ")] private StageDataBase _stageDataBase;
    [SerializeField, Tooltip("エネミー")] private EnemyDataBase _enemyDataBase;
    [SerializeField, Tooltip("マップデータ")] private MapData _mapData;
    [SerializeField, Tooltip("プレイヤーデータ")] private PlayerDataBase _playerDataBase;
    [SerializeField, Tooltip("生成マップデータ")] private GenerateMapData _generateMapData;
    [SerializeField, Tooltip("キーワードデータ")] private KeywordDataBase _keywordDataBase;

    [Header("ID")]
    [SerializeField, Tooltip("今のステージID")] public int StageID = 1;

    public BattlePhase CurrentPhase;
    public bool Reset = false, IsEnemyAction = false, DecreaseBuff = false;
    public StagePlayer Player;

    [NonSerialized] public UIManagerBase CurrentUIManager;
    [NonSerialized] public AttackManager AttackManager;
    [NonSerialized] public StageManager StageManager;
    [NonSerialized] public EffectManager EffectManager;

    private IBattleUI _uiManagerButtle;
    private AttackManager _attackManager;
    private DeckManager _deckManager;
    private FadeManager _fadeManager;
    private PlayerType _playerType = PlayerType.Combo;
    private PlayerStatus _playerStatus = null;
    private bool _isOrganize = false, _isDraw = false, _isAction = false, _isReward = false, _isBattleUIManager, _isGameover = false;

    [SerializeField] private SceneType _currentScene;

    private void Start()
    {
        Application.targetFrameRate = 60;
        _generateMapData = MapGenerator.GenerateMap(_mapData);
        _fadeManager = FadeManager.Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        if(_currentScene == SceneType.InGameScene)
        {
            Debug.Log(_playerStatus);
            CurrentPhase = BattlePhase.BuildStage;
        }
    }

    void Update()
    {
        if (_currentScene != SceneType.InGameScene) return;

        switch (CurrentPhase)
        {
            case BattlePhase.BuildStage: UpdateBuildStage(); break;
            case BattlePhase.Draw: UpdateDraw(); break;
            case BattlePhase.Set: break;
            case BattlePhase.Action: UpdateAction(); break;
            case BattlePhase.Direction: break;
            case BattlePhase.End: UpdateEnd(); break;
            case BattlePhase.Reward: UpdateReward(); break;
            case BattlePhase.Gameover: UpdateGameover(); break;
        }
    }

    /// <summary>
    /// ステージ生成フェーズ
    /// </summary>
    void UpdateBuildStage()
    {
        if (StageManager == null) return;

        TrySetPlayerStatus(true);
        Player.StagePlayerInit(_playerStatus);
        StageManager.CreateStage(StageID);
        _deckManager = DeckManager.Instance;
        _isReward = false;
        _isGameover = false;

        if (CurrentUIManager.TryGetComponent<IBattleUI>(out var battle))
            _uiManagerButtle = battle;

        CurrentPhase = BattlePhase.Draw;
    }

    /// <summary>
    /// ドローフェーズ
    /// </summary>
    void UpdateDraw()
    {
        if (!_isDraw)
        {
            //ドローバフの確認
            if (Player.HasBuff(BuffType.CardPlus))
                _uiManagerButtle.ChangeDrawCount(Player.GetBuffCount(BuffType.CardPlus));

            _uiManagerButtle.ShuffleDeck();
            StartCoroutine(_uiManagerButtle.DrawCardAnimation());
            Player.SetCost();
            _isDraw = true;
        }

        if (!_isOrganize)
        {
            _uiManagerButtle.HandOrganize();
            _isOrganize = true;
        }

        if (_isDraw && _isOrganize)
        {
            CurrentPhase = BattlePhase.Set;
        }
    }

    void UpdateAction()
    {
        if (!_isAction)
        {
            _uiManagerButtle.ResetDrawCount();
            _uiManagerButtle.ClearCard();
            CriAudioManager.Instance.PlaySe("SE_MagicShot");
            _attackManager = FindAnyObjectByType<AttackManager>();
            _attackManager.SwichTurn(true);
            StartCoroutine(_attackManager.AttackTurn(true));
            Player.SkeletonAnimation.AnimationState.SetAnimation(0, "攻撃モーション", false);
            Player.SkeletonAnimation.AnimationState.AddAnimation(0, "アイドルモーション", true, 0);
            _isAction = true;
        }

        if (IsEnemyAction)
        {
            _attackManager.SwichTurn(false);
            StartCoroutine(_attackManager.EnemyTurn());
            IsEnemyAction = false;
        }

        if (Reset)
        {
            Reset = false;
            CurrentPhase = BattlePhase.End;
        }
    }

    void UpdateEnd()
    {
        if (!DecreaseBuff)
        {
            Player.DecreaseAll();
            foreach (var enemy in StageManager.EnemyList)
                enemy.DecreaseAll();
            DecreaseBuff = true;
        }

        InitializeBool();
        CurrentPhase = BattlePhase.Draw;
    }

    void UpdateReward()
    {
        if (!_isReward)
        {
            _uiManagerButtle.DisplayReward();
            _isReward = true;
            InitializeBool();
            _playerStatus.SetCurrentHp(Player.CurrentHP);
        }
    }

    void UpdateGameover()
    {
        if (!_isGameover)
        {
            _isGameover = true;
            CurrentUIManager.GetComponent<UIManager_Battle>().DisplayGameOverPanel();
        }
    }


    public void RegisterUIManager(UIManagerBase ui)
    {
        CurrentUIManager = ui;
        CurrentUIManager.InitUI();
    }

    public void SceneChange(SceneType sceneType)
    {
        if (_fadeManager == null) _fadeManager = FadeManager.Instance;
        _fadeManager.FadePanel(false, async () =>
        {
            await SceneManager.LoadSceneAsync($"{sceneType}");
            _currentScene = sceneType;
            _fadeManager.FadePanel(true);
            if (sceneType == SceneType.TitleScene)
            {
                InitializeBool();
                TrySetPlayerStatus(false);
            }
        });
    }

    private void InitializeBool()
    {
        Reset = false;
        _isDraw = false;
        _isOrganize = false;
        _isAction = false;
        DecreaseBuff = false;
        IsEnemyAction = false;
    }

    private void TrySetPlayerStatus(bool isSet)
    {
        if(isSet)
        {
            if (_playerStatus != null) return;
            PlayerData playerData = _playerDataBase.GetPlayerData(_playerType);
            _playerStatus = new PlayerStatus(_playerType, playerData.PlayerMaxHp, playerData.PlayerMaxCost);
        }
        else
        {
            _playerStatus = null;
        }
    }

    /// <summary>
    /// プレイヤーのキャラクターを変更
    /// </summary>
    /// <param name="useType"></param>
    public void ChangePlayerType(PlayerType useType)
    {
        _playerType = useType;
    }
    /// <summary>
    /// プレイヤーのキャラによって特定バフを付与する
    /// </summary>
    /// <param name="playerType">プレイヤーのタイプ</param>
    /// <returns>タイプ毎の特殊バフ</returns>
    public BuffData GivePlayerBuffData()
    {
        return _buffDataBase.GetBuffData((BuffType)_playerType);
    }

}