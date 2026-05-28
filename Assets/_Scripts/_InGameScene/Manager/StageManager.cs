using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public List<List<GameObject>> SlotList => _slotList;
    public List<Enemy> EnemyList => _enemyList;
    public StageData Stage => _stage;

    [Header("ステージの情報")]
    [SerializeField, Tooltip("ステージのタイルリスト")] private List<List<GameObject>> _slotList = new List<List<GameObject>>();
    [SerializeField, Tooltip("ステージのエネミーリスト")] private List<Enemy> _enemyList = new();

    [Header("コンポーネント設定")]
    [SerializeField, Tooltip("生成するタイル")] private GameObject _tailPrefab;
    [SerializeField, Tooltip("背景の画像")] private Image _backGroundImage;
    [SerializeField, Tooltip("生成するエネミースロット")] private GameObject _enemySlot;
    [SerializeField, Tooltip("エネミーのパネル")] private GameObject _enemyPanel;
    [SerializeField, Tooltip("攻撃場所選択パネル")] private GameObject _serectPanel;
    [SerializeField, Tooltip("攻撃場所選択ボタン")] private GameObject _attackPointButton;

    [Header("タイルの幅")]
    [SerializeField, Tooltip("横幅")] private float _widthSize = 0.8f;
    [SerializeField, Tooltip("縦幅")] private float _heightSize = 0.8f;

    private GridLayoutGroup _layoutGroup;
    private StageData _stage;
    private Transform _parent;
    private Transform _enemyParent;
    private GameObject _slot;
    private GameObject _enemy,_button;


    private void Awake()
    {
        GameManager.Instance.StageManager = this;
    }
    
    /// <summary>
    /// ゲーム内のステージ生成
    /// </summary>
    /// <param name="stageIndex"></param>
    public void CreateStage(int stageIndex)
    {
        _layoutGroup = GetComponent<GridLayoutGroup>();
        _stage = GameManager.Instance.StageDataBase.GetStageData(stageIndex);
        _layoutGroup.constraintCount = _stage.Width;
        _backGroundImage.sprite = _stage.Background;
        _parent = this.transform;
        _enemyParent = _enemyPanel.transform;
        _serectPanel.gameObject.SetActive(true);
        SlotList.Clear();
        AdjustCellSize();
        
        for (int i = 0; i < _stage.Height; i++)
        {
            List<GameObject> slotListH = new();
            for(int j = 0; j < _stage.Width; j++)
            {
                _slot = Instantiate(_tailPrefab, Vector3.zero, Quaternion.identity, _parent);
                _slot.name = ($"Slot{i},{j}");
                slotListH.Add(_slot);
            }
            _slotList.Add(slotListH);
        }

        AttackPointManager attackPointManager = FindAnyObjectByType<AttackPointManager>();

        for (int i = 0; i < _stage.Height; i++)
        {
            //_enemy = Instantiate(_enemySlot, Vector3.zero, Quaternion.identity, _enemyParent);
            //_enemyList.Add(_enemy.GetComponent<Enemy>());

            _button = Instantiate(_attackPointButton, Vector3.zero, Quaternion.identity, _serectPanel.transform);
            AttackPointSelectButton attackButton = _button.GetComponent<AttackPointSelectButton>();
            attackPointManager.AttackPointButtonList.Add(attackButton);
            attackButton.AttackNumber = i;
        }

        attackPointManager.CheckStartAttackPosition();

        for (int i = 0; i < _stage.Enemies.Length; i++)
        {
            _enemyList[_stage.Enemies[i].EnemyPosition].SetEnemyStatus(_stage.Enemies[i].EnemyID);
        }

        //空の敵を設置
        for(int i = 0; i < _enemyList.Count; i++)
        {
            if (IsEnemy(i)) continue;
            _enemyList[i].SetEnemyStatus(0);
        }
    }

    /// <summary>
    /// 指定の場所にエネミーが存在しているか
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool IsEnemy(int index)
    {
        for(int i = 0; i < _stage.Enemies.Length; i++)
        {
            if(index == _stage.Enemies[i].EnemyPosition)return true;
        }
        return false;
    }

    /// <summary>
    /// ステージを中央に寄せる
    /// </summary>
    private void AdjustCellSize()
    {
        RectTransform _tailePanelRect = GetComponent<RectTransform>();

        float bgWidth = _tailePanelRect.rect.width;   
        float bgHeight = _tailePanelRect.rect.height; 

        float cellWidth = bgWidth / _stage.Width;
        float cellHeight = bgHeight / _stage.Height;

        _layoutGroup.cellSize = new Vector2(cellWidth * _widthSize, cellHeight * _heightSize);
    }
}
