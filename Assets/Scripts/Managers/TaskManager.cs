using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class NotificationBadge
    {
        public GameObject badgeObject;
        public TextMeshProUGUI countText;
    }


    [Header("Notification Badges")]
    [SerializeField] private NotificationBadge emailBadge;
    [SerializeField] private NotificationBadge callBadge;
    [SerializeField] private NotificationBadge printBadge;
    [SerializeField] private NotificationBadge meetingBadge;


    private List<TaskData> activeTasks = new List<TaskData>();
    private List<TaskData> completedTasks = new List<TaskData>();
    private List<TaskData> failedTasks = new List<TaskData>();


    private int emailCount;
    private int callCount;
    private int printCount;
    private int meetingCount;


    private void Start()
    {
        UpdateNotificationBadges();
    }


    public void AddTask(TaskData task)
    {
        if (task == null)
        {
            Debug.LogWarning("Trying to add a null task.");
            return;
        }

        if (activeTasks.Contains(task))
        {
            return;
        }

        activeTasks.Add(task);

        IncreaseNotificationCount(task.taskType);

        UpdateNotificationBadges();

        Debug.Log("New task added: " + task.taskTitle);
    }


    public void CompleteTask(TaskData task)
    {
        if (task == null)
            return;

        if (!activeTasks.Contains(task))
            return;

        activeTasks.Remove(task);
        completedTasks.Add(task);

        DecreaseNotificationCount(task.taskType);

        UpdateNotificationBadges();

        Debug.Log("Task completed: " + task.taskTitle);
    }


    public void FailTask(TaskData task)
    {
        if (task == null)
            return;

        if (!activeTasks.Contains(task))
            return;

        activeTasks.Remove(task);
        failedTasks.Add(task);

        DecreaseNotificationCount(task.taskType);

        UpdateNotificationBadges();

        Debug.Log("Task failed: " + task.taskTitle);
    }


    private void IncreaseNotificationCount(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.Email:
                emailCount++;
                break;

            case TaskType.Call:
                callCount++;
                break;

            case TaskType.Print:
                printCount++;
                break;

            case TaskType.Meeting:
                meetingCount++;
                break;
        }
    }


    private void DecreaseNotificationCount(TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.Email:
                emailCount = Mathf.Max(0, emailCount - 1);
                break;

            case TaskType.Call:
                callCount = Mathf.Max(0, callCount - 1);
                break;

            case TaskType.Print:
                printCount = Mathf.Max(0, printCount - 1);
                break;

            case TaskType.Meeting:
                meetingCount = Mathf.Max(0, meetingCount - 1);
                break;
        }
    }


    private void UpdateNotificationBadges()
    {
        UpdateBadge(emailBadge, emailCount);
        UpdateBadge(callBadge, callCount);
        UpdateBadge(printBadge, printCount);
        UpdateBadge(meetingBadge, meetingCount);
    }


    private void UpdateBadge(
        NotificationBadge badge,
        int count)
    {
        if (badge == null || badge.badgeObject == null)
            return;

        // Update count.
        if (badge.countText != null)
        {
            badge.countText.text = count.ToString();
        }

        // Show badge only when there are notifications.
        badge.badgeObject.SetActive(count > 0);
    }

    // -------------- GETTERS ----------------
    public List<TaskData> GetActiveTasksList() 
    {
        return activeTasks;
    }

    public List<TaskData> GetCompletedTasksList()
    {
        return completedTasks;
    }

    public List<TaskData> GetFailedTasksList()
    {
        return failedTasks;
    }

    public int GetActiveTasksCount()
    {
        return activeTasks.Count;
    }

    public int GetCompletedTasksCount()
    {
        return completedTasks.Count;
    }

    public int GetFailedTasksCount()
    {
        return failedTasks.Count;
    }
}