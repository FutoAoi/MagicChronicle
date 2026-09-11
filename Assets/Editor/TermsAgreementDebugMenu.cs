using UnityEditor;
using UnityEngine;

public static class TermsAgreementDebugMenu
{
    [MenuItem("Tools/Debug/利用規約同意フラグをリセット")]
    private static void ResetAgreement()
    {
        PlayerPrefs.DeleteKey(TermsAgreementPanel.PrefsKeyAgreedVersion);
        PlayerPrefs.Save();
        Debug.Log("[TermsAgreementDebugMenu] 利用規約同意フラグをリセットしました");
    }
}
