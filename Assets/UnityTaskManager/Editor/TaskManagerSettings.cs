using UnityEngine;
using UnityEditor;

namespace TaskManager
{
    [System.Serializable]
    public class SyncSettings
    {
        public string spreadsheetId = "";
        public string gasUrl        = "";   // GAS WebアプリのデプロイURL
    }

    public class TaskManagerSettings : EditorWindow
    {
        private SyncSettings settings;

        private static readonly Color BgColor    = new Color(0.13f, 0.14f, 0.16f);
        private static readonly Color AccentColor = new Color(0.27f, 0.73f, 0.56f);
        private static readonly Color TextColor   = new Color(0.88f, 0.90f, 0.92f);
        private static readonly Color SubColor    = new Color(0.55f, 0.60f, 0.65f);

        [MenuItem("Tools/Task Manager Settings")]
        public static void Open()
        {
            var win = GetWindow<TaskManagerSettings>("⚙ Task Manager Settings");
            win.minSize = new Vector2(500, 320);
            win.settings = Load();
        }

        private void OnGUI()
        {
            if (settings == null) settings = Load();

            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), BgColor);
            GUILayout.Space(16);

            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 16,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = AccentColor }
            };
            GUILayout.Label("Google Sheets 連携設定（GAS方式）", titleStyle);
            GUILayout.Space(16);

            var labelStyle = new GUIStyle(EditorStyles.label)
            {
                normal    = { textColor = TextColor },
                fontStyle = FontStyle.Bold
            };
            var subStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal   = { textColor = SubColor },
                wordWrap = true
            };

            // ── Spreadsheet ID ───────────────────────────────
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("📄 スプレッドシートID", labelStyle);
                GUILayout.Label("GoogleスプレッドシートのURLの以下の部分をコピーしてください。\n" +
                                "https://docs.google.com/spreadsheets/d/【ここ】/edit", subStyle);
                GUILayout.Space(4);
                settings.spreadsheetId = EditorGUILayout.TextField(settings.spreadsheetId);
            }

            GUILayout.Space(10);

            // ── GAS URL ──────────────────────────────────────
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("🔗 GAS WebアプリURL", labelStyle);
                GUILayout.Label("スプレッドシートの「拡張機能 → Apps Script」でスクリプトを登録し、\n" +
                                "デプロイ → 新しいデプロイ → ウェブアプリ で発行されたURLを入力してください。", subStyle);
                GUILayout.Space(4);
                settings.gasUrl = EditorGUILayout.TextField(settings.gasUrl);
            }

            GUILayout.Space(10);

            // ── GAS Script hint ──────────────────────────────
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("📝 GASに貼り付けるコード", labelStyle);
                GUILayout.Label("Apps Scriptエディタに以下のコードをそのまま貼り付けてデプロイしてください。", subStyle);
                GUILayout.Space(4);

                var codeStyle = new GUIStyle(EditorStyles.textArea)
                {
                    fontSize = 10,
                    normal   = { textColor = new Color(0.7f, 0.9f, 0.7f) }
                };
                EditorGUILayout.SelectableLabel(GasScriptTemplate(), codeStyle, GUILayout.Height(60));
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("  保存  ", GUILayout.Height(32), GUILayout.Width(120)))
                {
                    Save(settings);
                    EditorUtility.DisplayDialog("保存完了", "設定を保存しました。", "OK");
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(16);
        }

        private static string GasScriptTemplate() =>
@"function doGet(e) {
  var s = SpreadsheetApp.openById('【スプレッドシートID】').getSheetByName('Tasks');
  var data = s.getDataRange().getValues();
  return ContentService.createTextOutput(JSON.stringify(data))
    .setMimeType(ContentService.MimeType.JSON);
}
function doPost(e) {
  if (e.parameter.action === 'write') {
    writeSheet(JSON.parse(e.parameter.data));
  }
  return ContentService.createTextOutput('ok')
    .setMimeType(ContentService.MimeType.TEXT);
}
function writeSheet(rows) {
  var s = SpreadsheetApp.openById('【スプレッドシートID】').getSheetByName('Tasks');
  s.clearContents();
  s.appendRow(['id','title','description','assignee','priority','isCompleted','createdAt','completedAt','dueDate']);
  rows.forEach(function(r) {
    s.appendRow([r.id,r.title,r.description,r.assignee,r.priority,r.isCompleted,r.createdAt,r.completedAt,r.dueDate]);
  });
}";

        // ─── Persistence ────────────────────────────────────
        private const string PrefKey = "TaskManager_Settings";

        public static SyncSettings Load()
        {
            string json = EditorPrefs.GetString(PrefKey, "");
            if (!string.IsNullOrEmpty(json))
                try { return JsonUtility.FromJson<SyncSettings>(json); } catch { }
            return new SyncSettings();
        }

        private static void Save(SyncSettings s) =>
            EditorPrefs.SetString(PrefKey, JsonUtility.ToJson(s));
    }
}
