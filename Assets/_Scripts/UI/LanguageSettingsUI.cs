using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

/// <summary>
/// 言語設定UI。
/// Dropdownで言語を選択、またはボタンから SetLanguage("ja") / SetLanguage("en") を呼んで言語を切り替えます。
/// 選択した言語は Localization Settings の Player Pref Locale Selector により自動で保存され、次回起動時に復元されます。
///
/// 使い方:
/// 1. Canvas上にTMP_Dropdownを用意し、このコンポーネントをアタッチしてInspectorから割り当てる。
///    （ボタンで切り替える場合はDropdown未設定のまま、ButtonのOnClickに SetLanguage を登録して "ja" / "en" を指定）
/// 2. カード名などデータから表示している文字は、表示し直すまで切り替わらない。
///    切り替え後すぐ反映させたいシーン（タイトル等）では _reloadSceneOnChange をONにする。
/// </summary>
public class LanguageSettingsUI : MonoBehaviour
{
    [Header("Dropdownの参照（ボタンで切り替える場合は未設定でOK）")]
    [SerializeField] private TMP_Dropdown _dropdown;

    [Header("切り替え後の挙動")]
    [Tooltip("言語切り替え後に現在のシーンを読み直すか（ラン途中のシーンではOFFにすること）")]
    [SerializeField] private bool _reloadSceneOnChange = false;

    private bool _isInitialized;

    private void OnEnable()
    {
        StartCoroutine(Initialize());
    }

    private void OnDisable()
    {
        if (_dropdown != null) _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        _isInitialized = false;
    }

    /// <summary>Localizationの初期化を待ってから、Dropdownに言語一覧を反映します。</summary>
    private IEnumerator Initialize()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (_dropdown != null)
        {
            _dropdown.ClearOptions();
            foreach (Locale locale in LocalizationSettings.AvailableLocales.Locales)
            {
                // 言語名はその言語自身の表記で表示（例："日本語" / "English"）
                _dropdown.options.Add(new TMP_Dropdown.OptionData(GetDisplayName(locale)));
            }
            UpdateDropdownValue();
            _dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        _isInitialized = true;
    }

    /// <summary>
    /// 言語コードを指定して切り替えます（ButtonのOnClickから呼ぶ用）。
    /// </summary>
    /// <param name="code">言語コード（"ja" / "en"）</param>
    public void SetLanguage(string code)
    {
        if (!_isInitialized) return;

        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(code);
        if (locale == null)
        {
            Debug.LogWarning($"[LanguageSettingsUI] 言語 '{code}' が Available Locales にありません");
            return;
        }
        ChangeLocale(locale);
    }

    private void OnDropdownChanged(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (index < 0 || index >= locales.Count) return;
        ChangeLocale(locales[index]);
    }

    private void ChangeLocale(Locale locale)
    {
        if (locale == LocalizationSettings.SelectedLocale) return;

        LocalizationSettings.SelectedLocale = locale;

        if (_reloadSceneOnChange)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /// <summary>他の場所（Localization Scene Controls等）で言語が変わった場合もDropdownの表示を合わせます。</summary>
    private void OnSelectedLocaleChanged(Locale locale)
    {
        UpdateDropdownValue();
    }

    private void UpdateDropdownValue()
    {
        if (_dropdown == null) return;

        int index = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        _dropdown.SetValueWithoutNotify(Mathf.Max(0, index));
        _dropdown.RefreshShownValue();
    }

    private static string GetDisplayName(Locale locale)
    {
        var culture = locale.Identifier.CultureInfo;
        return culture != null ? culture.NativeName : locale.LocaleName;
    }
}
