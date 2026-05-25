using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    // 列順: title | description | assignee | priority | isCompleted
    public static class SpreadsheetSync
    {
        public static void Sync(
            List<TaskData> localTasks,
            Action<List<TaskData>> onComplete,
            Action<string> onError)
        {
            var settings = TaskManagerSettings.Load();

            if (string.IsNullOrEmpty(settings.gasUrl))
            {
                onError?.Invoke("GAS WebアプリURLが設定されていません。\n⚙ 設定から入力してください。");
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    var remote = await FetchViaGas(settings.gasUrl);
                    var merged = MergeTasks(localTasks, remote);
                    await WriteViaGas(settings.gasUrl, merged);
                    EditorApplication.delayCall += () => onComplete?.Invoke(merged);
                }
                catch (Exception ex)
                {
                    EditorApplication.delayCall += () => onError?.Invoke(ex.Message);
                }
            });
        }

        // ─── GAS GET ──────────────────────────────────────────────────
        private static async Task<List<TaskData>> FetchViaGas(string gasUrl)
        {
            using var http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(20);
            var response = await http.GetAsync(gasUrl);
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return ParseGasJson(json);
        }

        // ─── JSON パース [[row],[row],...] ────────────────────────────
        private static List<TaskData> ParseGasJson(string json)
        {
            var tasks = new List<TaskData>();
            if (string.IsNullOrWhiteSpace(json) || json == "[]") return tasks;

            var rows = SplitJsonRows(json);
            if (rows.Count < 2) return tasks; // ヘッダーのみ

            for (int i = 1; i < rows.Count; i++)
            {
                var cols = rows[i];
                if (string.IsNullOrWhiteSpace(SafeGet(cols, 0))) continue;
                tasks.Add(new TaskData
                {
                    title       = SafeGet(cols, 0),
                    description = SafeGet(cols, 1),
                    assignee    = SafeGet(cols, 2),
                    priority    = ParsePriority(SafeGet(cols, 3)),
                    isCompleted = SafeGet(cols, 4).Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                });
            }
            return tasks;
        }

        // ─── GAS POST ─────────────────────────────────────────────────
        private static async Task WriteViaGas(string gasUrl, List<TaskData> tasks)
        {
            var sb = new StringBuilder("[");
            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                sb.Append("{")
                  .Append($"\"title\":\"{Esc(t.title)}\",")
                  .Append($"\"description\":\"{Esc(t.description)}\",")
                  .Append($"\"assignee\":\"{Esc(t.assignee)}\",")
                  .Append($"\"priority\":\"{t.priority}\",")
                  .Append($"\"isCompleted\":\"{(t.isCompleted ? "TRUE" : "FALSE")}\"")
                  .Append(i < tasks.Count - 1 ? "}," : "}");
            }
            sb.Append("]");

            using var http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(30);
            var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", "write"),
                new KeyValuePair<string, string>("data",   sb.ToString())
            });
            var response = await http.PostAsync(gasUrl, form);
            response.EnsureSuccessStatusCode();
        }

        // ─── マージ（シート優先）─────────────────────────────────────
        // タイトル+担当者 をキーに突き合わせる（idがないため）
        private static List<TaskData> MergeTasks(List<TaskData> local, List<TaskData> remote)
        {
            // シート側をベースにして、ローカルの完了状態を反映
            var remoteKeys = remote.Select(TaskKey).ToHashSet();
            var merged = new List<TaskData>(remote);

            // ローカルの完了フラグをシート側に上書き
            foreach (var rt in merged)
            {
                var lt = local.FirstOrDefault(t => TaskKey(t) == TaskKey(rt));
                if (lt != null && lt.isCompleted)
                    rt.isCompleted = true;
            }

            // ローカルのみの新規タスクを追加
            foreach (var lt in local)
                if (!remoteKeys.Contains(TaskKey(lt)))
                    merged.Add(lt);

            return merged.OrderBy(t => t.assignee).ThenBy(t => t.title).ToList();
        }

        private static string TaskKey(TaskData t) => $"{t.assignee}|{t.title}";

        // ─── ユーティリティ ───────────────────────────────────────────
        private static List<List<string>> SplitJsonRows(string json)
        {
            var result  = new List<List<string>>();
            int depth   = 0;
            bool inStr  = false;
            bool escape = false;
            var cell    = new StringBuilder();
            List<string> row = null;

            foreach (char c in json)
            {
                if (escape) { cell.Append(c); escape = false; continue; }
                if (c == '\\') { escape = true; continue; }
                if (c == '"') { inStr = !inStr; continue; }
                if (inStr)  { cell.Append(c); continue; }

                if (c == '[')
                {
                    depth++;
                    if (depth == 2) { row = new List<string>(); cell.Clear(); }
                }
                else if (c == ']')
                {
                    if (depth == 2 && row != null)
                    {
                        row.Add(cell.ToString().Trim());
                        cell.Clear();
                        result.Add(row);
                    }
                    depth--;
                }
                else if (c == ',' && depth == 2 && row != null)
                {
                    row.Add(cell.ToString().Trim());
                    cell.Clear();
                }
                else if (depth == 2)
                {
                    cell.Append(c);
                }
            }
            return result;
        }

        private static string SafeGet(List<string> list, int i) => i < list.Count ? list[i] : "";
        private static string Esc(string s) => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
        private static TaskPriority ParsePriority(string s) => s switch
        {
            "High" => TaskPriority.High,
            "Low"  => TaskPriority.Low,
            _      => TaskPriority.Medium
        };
    }
}
