using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager
{
    public class TaskManagerWindow : EditorWindow
    {
        // ─── State ───────────────────────────────────────────────
        private List<TaskData> tasks = new List<TaskData>();
        private string currentUser = "";
        private string newUserName = "";
        private string newTaskTitle = "";
        private string newTaskDescription = "";
        private TaskPriority newTaskPriority = TaskPriority.Medium;

        private Vector2 scrollPos;
        private Vector2 sidebarScrollPos;
        private bool isLoading = false;
        private bool isSyncing = false;
        private string statusMessage = "";
        private MessageType statusType = MessageType.None;
        private double statusClearTime = 0;

        private TaskFilter currentFilter = TaskFilter.All;
        private string searchQuery = "";

        // ─── Styles ──────────────────────────────────────────────
        private GUIStyle headerStyle;
        private GUIStyle sectionHeaderStyle;
        private GUIStyle taskCardStyle;
        private GUIStyle completedTaskStyle;
        private GUIStyle userButtonStyle;
        private GUIStyle activeUserButtonStyle;
        private GUIStyle priorityBadgeStyle;
        private bool stylesInitialized = false;

        // ─── Colors ──────────────────────────────────────────────
        private static readonly Color ColorBg         = new Color(0.13f, 0.14f, 0.16f);
        private static readonly Color ColorSidebar    = new Color(0.10f, 0.11f, 0.13f);
        private static readonly Color ColorCard       = new Color(0.18f, 0.20f, 0.23f);
        private static readonly Color ColorCardDone   = new Color(0.14f, 0.17f, 0.14f);
        private static readonly Color ColorAccent     = new Color(0.27f, 0.73f, 0.56f);
        private static readonly Color ColorAccentHigh = new Color(0.95f, 0.45f, 0.35f);
        private static readonly Color ColorAccentMed  = new Color(0.95f, 0.78f, 0.25f);
        private static readonly Color ColorAccentLow  = new Color(0.35f, 0.65f, 0.95f);
        private static readonly Color ColorText       = new Color(0.88f, 0.90f, 0.92f);
        private static readonly Color ColorSubText    = new Color(0.55f, 0.60f, 0.65f);
        private static readonly Color ColorDivider    = new Color(0.25f, 0.27f, 0.30f);

        // ─── Open ─────────────────────────────────────────────────
        [MenuItem("Tools/Task Manager %#t")]
        public static void Open()
        {
            var win = GetWindow<TaskManagerWindow>("📋 Task Manager");
            win.minSize = new Vector2(780, 500);
        }

        // ─── Lifecycle ────────────────────────────────────────────
        private void OnEnable()
        {
            LoadFromLocal();
            if (string.IsNullOrEmpty(currentUser) && tasks.Count > 0)
                currentUser = tasks.Select(t => t.assignee).Distinct().FirstOrDefault() ?? "";
        }

        private void OnGUI()
        {
            InitStyles();
            HandleKeyboard();

            // Auto-clear status
            if (statusClearTime > 0 && EditorApplication.timeSinceStartup > statusClearTime)
            {
                statusMessage = "";
                statusClearTime = 0;
                Repaint();
            }

            // Root background
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), ColorBg);

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSidebar();
                DrawMain();
            }

            if (isLoading || isSyncing)
                Repaint();
        }

        // ─── Sidebar ──────────────────────────────────────────────
        private void DrawSidebar()
        {
            float sidebarW = 200;
            // Draw sidebar background before entering the layout group
            EditorGUI.DrawRect(new Rect(0, 0, sidebarW, position.height), ColorSidebar);

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(sidebarW), GUILayout.ExpandHeight(true)))
            {

                GUILayout.Space(16);
                GUILayout.Label("TASK MANAGER", headerStyle);
                GUILayout.Space(4);
                DrawDivider();
                GUILayout.Space(12);

                // Sync button
                GUI.enabled = !isSyncing && !isLoading;
                if (DrawIconButton(isSyncing ? "⟳  同期中..." : "⟳  スプレッドシート同期", ColorAccent))
                    SyncWithSpreadsheet();
                GUI.enabled = true;

                GUILayout.Space(16);
                DrawDivider();
                GUILayout.Space(12);

                // Filter
                GUILayout.Label("フィルター", sectionHeaderStyle);
                GUILayout.Space(6);
                DrawFilterButton("すべて",    TaskFilter.All);
                DrawFilterButton("未完了",    TaskFilter.Incomplete);
                DrawFilterButton("完了済み",  TaskFilter.Completed);

                GUILayout.Space(16);
                DrawDivider();
                GUILayout.Space(12);

                // Users
                GUILayout.Label("メンバー", sectionHeaderStyle);
                GUILayout.Space(6);

                sidebarScrollPos = EditorGUILayout.BeginScrollView(sidebarScrollPos,
                    GUIStyle.none, GUIStyle.none, GUILayout.ExpandHeight(true));

                var users = tasks.Select(t => t.assignee).Distinct().OrderBy(u => u).ToList();
                foreach (var user in users)
                {
                    bool isActive = currentUser == user;
                    int total = tasks.Count(t => t.assignee == user);
                    int done  = tasks.Count(t => t.assignee == user && t.isCompleted);

                    var style = isActive ? activeUserButtonStyle : userButtonStyle;
                    if (GUILayout.Button($"  {user}  ({done}/{total})", style, GUILayout.ExpandWidth(true)))
                        currentUser = user;
                    GUILayout.Space(2);
                }

                EditorGUILayout.EndScrollView();

                GUILayout.Space(8);
                DrawDivider();
                GUILayout.Space(8);

                // Settings
                if (DrawIconButton("⚙  設定", ColorSubText))
                    TaskManagerSettings.Open();

                GUILayout.Space(12);
            }
        }

        // ─── Main Area ────────────────────────────────────────────
        private void DrawMain()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                DrawTopBar();
                DrawDivider();
                GUILayout.Space(8);

                // Status message
                if (!string.IsNullOrEmpty(statusMessage))
                {
                    var msgColor = statusType == MessageType.Error   ? new Color(0.9f, 0.3f, 0.3f) :
                                   statusType == MessageType.Warning ? new Color(0.9f, 0.7f, 0.2f) :
                                   ColorAccent;
                    var prevColor = GUI.color;
                    GUI.color = msgColor;
                    EditorGUILayout.HelpBox(statusMessage, statusType == MessageType.None ? MessageType.Info : statusType);
                    GUI.color = prevColor;
                    GUILayout.Space(6);
                }

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                DrawTaskList();
                EditorGUILayout.EndScrollView();

                DrawDivider();
                DrawAddTaskPanel();
            }
        }

        private void DrawTopBar()
        {
            GUILayout.Space(12);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(16);
                var users = tasks.Select(t => t.assignee).Distinct().OrderBy(u => u).ToList();
                string displayUser = string.IsNullOrEmpty(currentUser) ? "全員" : currentUser;
                GUILayout.Label($"📋  {displayUser} のタスク", new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 16,
                    normal = { textColor = ColorText }
                });

                GUILayout.FlexibleSpace();

                // Search
                GUILayout.Label("🔍", GUILayout.Width(20));
                searchQuery = EditorGUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField, GUILayout.Width(180));

                GUILayout.Space(16);
            }
            GUILayout.Space(10);
        }

        // ─── Task List ────────────────────────────────────────────
        private void DrawTaskList()
        {
            var filtered = GetFilteredTasks();

            if (filtered.Count == 0)
            {
                GUILayout.Space(40);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.Label("✅", new GUIStyle { fontSize = 48, alignment = TextAnchor.MiddleCenter });
                        GUILayout.Label(currentFilter == TaskFilter.Completed ? "完了済みタスクはありません" :
                                        currentFilter == TaskFilter.Incomplete ? "未完了タスクはありません！" :
                                        "タスクはありません", new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        { fontSize = 13, normal = { textColor = ColorSubText } });
                    }
                    GUILayout.FlexibleSpace();
                }
                return;
            }

            // Group by assignee if "全員"
            bool showAll = string.IsNullOrEmpty(currentUser);
            var groups = showAll
                ? filtered.GroupBy(t => t.assignee).ToList()
                : filtered.GroupBy(t => t.assignee).Where(g => g.Key == currentUser).ToList();

            foreach (var group in groups)
            {
                if (showAll)
                {
                    GUILayout.Space(4);
                    GUILayout.Label($"👤  {group.Key}", new GUIStyle(EditorStyles.boldLabel)
                    {
                        normal = { textColor = ColorSubText },
                        fontSize = 11
                    });
                    GUILayout.Space(4);
                }

                foreach (var task in group.OrderBy(t => t.isCompleted).ThenBy(t => (int)t.priority))
                    DrawTaskCard(task);

                GUILayout.Space(8);
            }

            GUILayout.Space(16);
        }

        private void DrawTaskCard(TaskData task)
        {
            var cardColor = task.isCompleted ? ColorCardDone : ColorCard;
            var rect = EditorGUILayout.BeginVertical(task.isCompleted ? completedTaskStyle : taskCardStyle);
            EditorGUI.DrawRect(rect, cardColor);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(12);

                // Checkbox
                bool newDone = EditorGUILayout.Toggle(task.isCompleted, GUILayout.Width(20));
                if (newDone != task.isCompleted)
                {
                    task.isCompleted = newDone;

                    SaveToLocal();
                    ShowStatus(newDone ? $"✅ \"{task.title}\" を完了しました" : $"↩ \"{task.title}\" を未完了に戻しました");
                }

                GUILayout.Space(6);

                // Priority badge
                DrawPriorityBadge(task.priority);
                GUILayout.Space(6);

                // Title & description
                using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                {
                    var titleStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        normal = { textColor = task.isCompleted ? ColorSubText : ColorText },
                        fontSize = 13
                    };
                    if (task.isCompleted)
                        titleStyle.fontStyle = FontStyle.Italic;

                    GUILayout.Label(task.isCompleted ? $"~~{task.title}~~" : task.title, titleStyle);

                    if (!string.IsNullOrEmpty(task.description))
                    {
                        GUILayout.Label(task.description, new GUIStyle(EditorStyles.miniLabel)
                        {
                            normal = { textColor = ColorSubText },
                            wordWrap = true
                        });
                    }

                    if (task.isCompleted)
                    {
                        GUILayout.Label("✅ 完了", new GUIStyle(EditorStyles.miniLabel)
                        {
                            normal = { textColor = new Color(0.4f, 0.7f, 0.4f) }
                        });
                    }
                }

                // Delete button
                GUI.color = new Color(1f, 1f, 1f, 0.3f);
                if (GUILayout.Button("✕", EditorStyles.miniLabel, GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("タスクの削除",
                        $"\"{task.title}\" を削除しますか？", "削除", "キャンセル"))
                    {
                        tasks.Remove(task);
                        SaveToLocal();
                        ShowStatus($"🗑 \"{task.title}\" を削除しました");
                    }
                }
                GUI.color = Color.white;

                GUILayout.Space(8);
            }

            GUILayout.Space(8);
            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        // ─── Add Task Panel ───────────────────────────────────────
        private void DrawAddTaskPanel()
        {
            GUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("新しいタスクを追加", sectionHeaderStyle);
                GUILayout.Space(6);

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(16);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            // User name
                            GUILayout.Label("担当者:", GUILayout.Width(52));
                            var users = tasks.Select(t => t.assignee).Distinct().OrderBy(u => u).ToList();
                            if (!string.IsNullOrEmpty(currentUser))
                                newUserName = currentUser;

                            newUserName = EditorGUILayout.TextField(newUserName, GUILayout.Width(120));

                            GUILayout.Space(12);

                            // Priority
                            GUILayout.Label("優先度:", GUILayout.Width(48));
                            newTaskPriority = (TaskPriority)EditorGUILayout.EnumPopup(newTaskPriority, GUILayout.Width(80));

                            GUILayout.FlexibleSpace();
                        }

                        GUILayout.Space(4);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Label("タイトル:", GUILayout.Width(52));
                            newTaskTitle = EditorGUILayout.TextField(newTaskTitle, GUILayout.ExpandWidth(true));
                            GUILayout.Space(8);

                            GUI.enabled = !string.IsNullOrWhiteSpace(newTaskTitle) && !string.IsNullOrWhiteSpace(newUserName);
                            if (GUILayout.Button("＋ 追加", GUILayout.Width(80)))
                                AddTask();
                            GUI.enabled = true;
                        }

                        GUILayout.Space(4);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Label("メモ:", GUILayout.Width(52));
                            newTaskDescription = EditorGUILayout.TextField(newTaskDescription, GUILayout.ExpandWidth(true));
                        }
                    }
                    GUILayout.Space(16);
                }
            }
            GUILayout.Space(12);
        }

        // ─── Helpers ──────────────────────────────────────────────
        private void AddTask()
        {
            var task = new TaskData
            {
                title       = newTaskTitle.Trim(),
                description = newTaskDescription.Trim(),
                assignee    = newUserName.Trim(),
                priority    = newTaskPriority,
                isCompleted = false
            };
            tasks.Add(task);

            currentUser = task.assignee;
            newTaskTitle       = "";
            newTaskDescription = "";

            SaveToLocal();
            ShowStatus($"✅ \"{task.title}\" を追加しました");
            GUI.FocusControl(null);
        }

        private List<TaskData> GetFilteredTasks()
        {
            return tasks.Where(t =>
            {
                bool userMatch   = string.IsNullOrEmpty(currentUser) || t.assignee == currentUser;
                bool filterMatch = currentFilter == TaskFilter.All ||
                                   (currentFilter == TaskFilter.Completed   &&  t.isCompleted) ||
                                   (currentFilter == TaskFilter.Incomplete  && !t.isCompleted);
                bool searchMatch = string.IsNullOrEmpty(searchQuery) ||
                                   t.title.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase) ||
                                   t.description.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase);
                return userMatch && filterMatch && searchMatch;
            }).ToList();
        }

        private void DrawPriorityBadge(TaskPriority priority)
        {
            string label = priority switch
            {
                TaskPriority.High   => "🔴 高",
                TaskPriority.Medium => "🟡 中",
                TaskPriority.Low    => "🔵 低",
                _ => ""
            };
            GUILayout.Label(label, new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 10,
                normal = { textColor = priority switch
                {
                    TaskPriority.High   => ColorAccentHigh,
                    TaskPriority.Medium => ColorAccentMed,
                    _                   => ColorAccentLow
                }}
            }, GUILayout.Width(36));
        }

        private void DrawFilterButton(string label, TaskFilter filter)
        {
            bool active = currentFilter == filter;
            var style   = active ? activeUserButtonStyle : userButtonStyle;
            if (GUILayout.Button(label, style, GUILayout.ExpandWidth(true)))
                currentFilter = filter;
            GUILayout.Space(2);
        }

        private bool DrawIconButton(string label, Color color)
        {
            var prev = GUI.color;
            GUI.color = color;
            bool clicked = GUILayout.Button(label, new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleLeft,
                padding   = new RectOffset(10, 10, 5, 5),
                normal    = { textColor = color }
            }, GUILayout.ExpandWidth(true));
            GUI.color = prev;
            return clicked;
        }

        private void DrawDivider()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, ColorDivider);
        }

        private void HandleKeyboard()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return &&
                !string.IsNullOrWhiteSpace(newTaskTitle) && !string.IsNullOrWhiteSpace(newUserName))
            {
                AddTask();
                e.Use();
            }
        }

        private void ShowStatus(string msg, MessageType type = MessageType.None)
        {
            statusMessage   = msg;
            statusType      = type;
            statusClearTime = EditorApplication.timeSinceStartup + 3.0;
            Repaint();
        }

        // ─── Sync ─────────────────────────────────────────────────
        private void SyncWithSpreadsheet()
        {
            isSyncing = true;
            ShowStatus("⟳  スプレッドシートと同期中...");
            SpreadsheetSync.Sync(tasks,
                onComplete: updatedTasks =>
                {
                    tasks     = updatedTasks;
                    isSyncing = false;
                    SaveToLocal();
                    ShowStatus("✅ 同期が完了しました");
                    Repaint();
                },
                onError: err =>
                {
                    isSyncing = false;
                    ShowStatus($"❌ 同期エラー: {err}", MessageType.Error);
                    Repaint();
                });
        }

        // ─── Local Persistence ────────────────────────────────────
        private const string PrefKey = "TaskManager_Tasks";

        private void SaveToLocal()
        {
            var wrapper = new TaskListWrapper { tasks = tasks };
            EditorPrefs.SetString(PrefKey, JsonUtility.ToJson(wrapper));
        }

        private void LoadFromLocal()
        {
            string json = EditorPrefs.GetString(PrefKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var wrapper = JsonUtility.FromJson<TaskListWrapper>(json);
                    tasks = wrapper?.tasks ?? new List<TaskData>();
                }
                catch { tasks = new List<TaskData>(); }
            }
        }

        // ─── Style Init ───────────────────────────────────────────
        private void InitStyles()
        {
            if (stylesInitialized) return;
            stylesInitialized = true;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 11,
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = ColorAccent }
            };

            sectionHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 10,
                normal   = { textColor = ColorSubText }
            };

            taskCardStyle = new GUIStyle(EditorStyles.helpBox)
            {
                margin  = new RectOffset(8, 8, 2, 2),
                padding = new RectOffset(0, 0, 6, 6)
            };

            completedTaskStyle = new GUIStyle(taskCardStyle);

            userButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                alignment = TextAnchor.MiddleLeft,
                padding   = new RectOffset(10, 10, 4, 4),
                normal    = { textColor = ColorSubText }
            };

            activeUserButtonStyle = new GUIStyle(userButtonStyle)
            {
                fontStyle = FontStyle.Bold,
                normal    = { textColor = ColorAccent }
            };

            priorityBadgeStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize  = 10,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
