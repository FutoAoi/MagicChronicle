using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SceneType _sceneType;
    [SerializeField] private TermsAgreementPanel _termsAgreementPanel;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_termsAgreementPanel != null && !_termsAgreementPanel.HasAgreed)
        {
            _termsAgreementPanel.Show();
            return;
        }

        CriAudioManager.Instance.PlaySe("SE_TitleClick");
        GameManager.Instance.SceneChange(_sceneType);
    }
}
