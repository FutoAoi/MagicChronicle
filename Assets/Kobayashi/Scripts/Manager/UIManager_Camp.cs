using UnityEngine;

public class UIManager_Camp : UIManagerBase
{
    [Header("参照")]
    [SerializeField, Tooltip("初期表示UI")] private GameObject[] _objs; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void InitUI()
    {
        ChangeActiveObject(true);
    }
    /// <summary>
    /// 初期表示UIの表示/非表示
    /// </summary>
    /// <param name="show">見せるならtrue</param>
    public void ChangeActiveObject(bool show)
    {
        foreach (var obj in _objs)
        {
            obj.SetActive(show);
        }
    }

    public override void UpdateCostUI()
    {
        throw new System.NotImplementedException();
    }
}
