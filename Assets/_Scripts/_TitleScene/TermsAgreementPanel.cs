using UnityEngine;
using UnityEngine.UI;

public class TermsAgreementPanel : MonoBehaviour
{
    public const string PrefsKeyAgreedVersion = "AgreedTermsVersion";
    private const int CurrentTermsVersion = 1;

    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private Button _agreeButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _termsLinkButton;
    [SerializeField] private string _termsUrl = "https://kobayashitetsuya-65.github.io/magicchronicle-privacy/terms.html";

    public bool HasAgreed => PlayerPrefs.GetInt(PrefsKeyAgreedVersion, 0) >= CurrentTermsVersion;

    private void Awake()
    {
        _panelRoot.SetActive(false);
        _agreeButton.onClick.AddListener(OnAgreeButtonClicked);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        _termsLinkButton.onClick.AddListener(OnTermsLinkButtonClicked);
    }
    public void Show()
    {
        _panelRoot.SetActive(true);
    }

    private void OnCloseButtonClicked()
    {
        _panelRoot.SetActive(false);
    }

    private void OnAgreeButtonClicked()
    {
        PlayerPrefs.SetInt(PrefsKeyAgreedVersion, CurrentTermsVersion);
        PlayerPrefs.Save();
        _panelRoot.SetActive(false);
    }

    private void OnTermsLinkButtonClicked()
    {
        Application.OpenURL(_termsUrl);
    }
}
