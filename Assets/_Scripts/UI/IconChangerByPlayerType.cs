using UnityEngine;

public class IconChangerByPlayerType : MonoBehaviour
{
    [SerializeField] private GameObject ConboIcon;
    [SerializeField] private GameObject BerserkerIcon;
    [SerializeField] private GameObject TechnicalIcon;

    private void Start()
    {

        switch (GameManager.Instance.PlayerType)
        {
            case PlayerType.Combo:
                ConboIcon.SetActive(true);
                break;
            case PlayerType.Berserker:
                BerserkerIcon.SetActive(true);
                break;
            case PlayerType.Technical:
                TechnicalIcon.SetActive(true);
                break;
        }
    }

}
