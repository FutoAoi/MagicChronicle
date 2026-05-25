using System;
using System.Collections.Generic;

namespace TaskManager
{
    [Serializable]
    public class TaskData
    {
        public string       title       = "";
        public string       description = "";
        public string       assignee    = "";
        public TaskPriority priority    = TaskPriority.Medium;
        public bool         isCompleted = false;
    }

    [Serializable]
    public class TaskListWrapper
    {
        public List<TaskData> tasks = new();
    }

    public enum TaskPriority
    {
        High   = 0,
        Medium = 1,
        Low    = 2
    }

    public enum TaskFilter
    {
        All,
        Incomplete,
        Completed
    }
}
