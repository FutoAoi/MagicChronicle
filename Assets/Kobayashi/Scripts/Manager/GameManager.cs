using System;
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

    [Header("データベース")]
    [SerializeField, Tooltip("カード")] private CardDataBase _cardDataBase;
    [SerializeField, Tooltip("バフ")] private BuffDataBase _buffDataBase;
    [SerializeField, Tooltip("ステージ")] private StageDataBase _stageDataBase;
    [SerializeField, Tooltip("エネミー")] private EnemyDataBase _enemyDataBase;
    [SerializeField, Tooltip("マップデータ")] private MapData _mapData;
    [SerializeField, Tooltip("生成マップデータ")] private GenerateMapData _generateMapData;

    [Header("ID")]
    [SerializeField, Tooltip("ステージID")] public int StageID = 1;

    public BattlePhase CurrentPhase;
    public bool Reset = false, IsEnemyAction = false;
    public StagePlayer Player;

    [NonSerialized] public UIManagerBase CurrentUIManager;
    [NonSerialized] public AttackManager AttackManager;
    [NonSerialized] public StageManager StageManager;

    private IBattleUI _uiManagerButtle;
    private AttackManager _attackManager;
    private FadeManager _fadeManager;
    private bool _isOrganize = false,_isDraw = false,_isAction = false,_isReward = false,
        _isBattleUIManager;

    [SerializeField]private SceneType _currentScene;

    private void Start()
    {
        Application.targetFrameRate = 60;
        _generateMapData = MapGenerator.GenerateMap(_mapData);
        _fadeManager = FadeManager.Instance;
        if(Player != null )
        {
            Player.SetStatus(10, 10);
        }
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
        CurrentPhase = BattlePhase.BuildStage;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_currentScene)
        {
            case SceneType.TitleScene:
                break;
            case SceneType.InGameScene:
                switch (CurrentPhase)
                {
                    case BattlePhase.BuildStage:
                        if (StageManager != null)
                        {
                            StageManager.CreateStage(StageID);
                            CurrentPhase = BattlePhase.Draw;
                            if(CurrentUIManager.TryGetComponent<IBattleUI>(out var Battle))
                            {
                                _uiManagerButtle = Battle;
                            }
                            _isReward = false;
                        }
                        break;
                    case BattlePhase.Draw:
                        if (!_isDraw)
                        {
                            DeckManager.Instance.ShuffleDeck();
                            StartCoroutine(_uiManagerButtle.DrawCard());
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
                            _uiManagerButtle.ChangeDrawCount();
                        }
                        break;
                    case BattlePhase.Set:

                        break;
                    case BattlePhase.Action:
                        if (!_isAction)
                        {
                            _attackManager = FindAnyObjectByType<AttackManager>();
                            _attackManager.SwichTurn(true);
                            _attackManager.AttackTurn(true);
                            _uiManagerButtle.ClearCard();
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
                            InitializeBool();
                            CurrentPhase = BattlePhase.Draw;
                        }
                        break;
                    case BattlePhase.Direction:

                        break;
                    case BattlePhase.Reward:
                        if (!_isReward)
                        {
                            _uiManagerButtle.DisplayReward();
                            _isReward = true;
                        }
                        InitializeBool();
                        break;
                }
                break;
            case SceneType.StageSerectScene:
                break;
            case SceneType.CampScene: 
                
                break;
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
            if(sceneType == SceneType.TitleScene)
            {
                InitializeBool();
            }
        });
    }

    private void InitializeBool()
    {
        Reset = false;
        _isDraw = false;
        _isOrganize = false;
        _isAction = false;
    }
}
