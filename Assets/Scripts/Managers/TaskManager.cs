using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
    [System.Serializable]
    public class NotificationBadge
    {
        public GameObject badgeObject;
        public TMP_Text countText;
    }


    [Header("Task Pools")]
    [SerializeField] private List<TaskData> emailTasks;
    [SerializeField] private List<TaskData> callTasks;
    [SerializeField] private List<TaskData> printTasks;
    [SerializeField] private List<TaskData> meetingTasks;


    [Header("Task Spawning")]
    [SerializeField] private int minimumDelayMinutes = 2;
    [SerializeField] private int maximumDelayMinutes = 5;

    [SerializeField] private int maximumActiveTasks = 3;


    [Header("Work Day")]
    [SerializeField] private int workStartHour = 9;
    [SerializeField] private int workStartMinute = 0;

    [SerializeField] private int workEndHour = 17;
    [SerializeField] private int workEndMinute = 0;


    [Header("Notification Badges")]
    [SerializeField] private NotificationBadge emailBadge;
    [SerializeField] private NotificationBadge callBadge;
    [SerializeField] private NotificationBadge printBadge;
    [SerializeField] private NotificationBadge meetingBadge;


    private List<RuntimeTask> activeTasks =
        new List<RuntimeTask>();

    private List<RuntimeTask> completedTasks =
        new List<RuntimeTask>();

    private List<RuntimeTask> failedTasks =
        new List<RuntimeTask>();


    private int emailCount;
    private int callCount;
    private int printCount;
    private int meetingCount;


    private int currentHour;
    private int currentMinute;

    private int nextTaskHour;
    private int nextTaskMinute;

    private bool workDayStarted;
    private bool workDayFinished;


    private void Start()
    {
        currentHour = workStartHour;
        currentMinute = workStartMinute;

        ScheduleNextTask();

        UpdateNotificationBadges();
    }


    // Called by TaskBarClock whenever the game time changes.
    public void SetCurrentTime(
        int hour,
        int minute)
    {
        currentHour = hour;
        currentMinute = minute;

        if (!workDayStarted)
        {
            workDayStarted = true;
        }

        if (IsAfterWorkHours())
        {
            workDayFinished = true;
            return;
        }

        CheckForNewTask();
        CheckForExpiredTasks();
    }


    private void CheckForNewTask()
    {
        if (!workDayStarted)
            return;

        if (workDayFinished)
            return;

        if (activeTasks.Count >= maximumActiveTasks)
            return;

        if (IsTimeReached(
            currentHour,
            currentMinute,
            nextTaskHour,
            nextTaskMinute))
        {
            GenerateRandomTask();

            ScheduleNextTask();
        }
    }


    private void ScheduleNextTask()
    {
        int delay = Random.Range(
            minimumDelayMinutes,
            maximumDelayMinutes + 1
        );

        int totalMinutes =
            currentHour * 60 +
            currentMinute +
            delay;

        nextTaskHour =
            (totalMinutes / 60) % 24;

        nextTaskMinute =
            totalMinutes % 60;
    }


    private void GenerateRandomTask()
    {
        List<TaskData> availableTasks =
            GetAllTaskData();

        if (availableTasks.Count == 0)
        {
            Debug.LogWarning(
                "No TaskData has been assigned to TaskManager."
            );

            return;
        }

        TaskData selectedTask =
            availableTasks[
                Random.Range(
                    0,
                    availableTasks.Count
                )
            ];

        RuntimeTask runtimeTask =
            new RuntimeTask(
                selectedTask,
                currentHour,
                currentMinute
            );

        activeTasks.Add(runtimeTask);

        IncreaseNotificationCount(
            selectedTask.taskType
        );

        UpdateNotificationBadges();

        Debug.Log(
            "NEW TASK: " +
            selectedTask.taskTitle +
            " | Type: " +
            selectedTask.taskType +
            " | Spawn: " +
            currentHour +
            ":" +
            currentMinute.ToString("00") +
            " | Deadline: " +
            runtimeTask.deadlineHour +
            ":" +
            runtimeTask.deadlineMinute.ToString("00")
        );
    }


    private List<TaskData> GetAllTaskData()
    {
        List<TaskData> allTasks =
            new List<TaskData>();

        allTasks.AddRange(emailTasks);
        allTasks.AddRange(callTasks);
        allTasks.AddRange(printTasks);
        allTasks.AddRange(meetingTasks);

        return allTasks;
    }


    public void CompleteTask(RuntimeTask task)
    {
        if (task == null)
            return;

        if (!activeTasks.Contains(task))
            return;

        task.isCompleted = true;

        activeTasks.Remove(task);

        completedTasks.Add(task);

        DecreaseNotificationCount(
            task.taskData.taskType
        );

        UpdateNotificationBadges();

        Debug.Log(
            "COMPLETED: " +
            task.taskData.taskTitle
        );
    }


    public void FailTask(RuntimeTask task)
    {
        if (task == null)
            return;

        if (!activeTasks.Contains(task))
            return;

        task.isFailed = true;

        activeTasks.Remove(task);

        failedTasks.Add(task);

        DecreaseNotificationCount(
            task.taskData.taskType
        );

        UpdateNotificationBadges();

        Debug.Log(
            "FAILED: " +
            task.taskData.taskTitle
        );
    }


    private void CheckForExpiredTasks()
    {
        for (int i = activeTasks.Count - 1; i >= 0; i--)
        {
            RuntimeTask task =
                activeTasks[i];

            if (!task.taskData.canExpire)
                continue;

            if (IsTimeReached(
                currentHour,
                currentMinute,
                task.deadlineHour,
                task.deadlineMinute))
            {
                FailTask(task);
            }
        }
    }


    private bool IsTimeReached(
        int currentHour,
        int currentMinute,
        int targetHour,
        int targetMinute)
    {
        int currentTotal =
            currentHour * 60 +
            currentMinute;

        int targetTotal =
            targetHour * 60 +
            targetMinute;

        return currentTotal >= targetTotal;
    }


    private bool IsAfterWorkHours()
    {
        int currentTotal =
            currentHour * 60 +
            currentMinute;

        int endTotal =
            workEndHour * 60 +
            workEndMinute;

        return currentTotal >= endTotal;
    }


    private void IncreaseNotificationCount(
        TaskType taskType)
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


    private void DecreaseNotificationCount(
        TaskType taskType)
    {
        switch (taskType)
        {
            case TaskType.Email:
                emailCount =
                    Mathf.Max(0, emailCount - 1);
                break;

            case TaskType.Call:
                callCount =
                    Mathf.Max(0, callCount - 1);
                break;

            case TaskType.Print:
                printCount =
                    Mathf.Max(0, printCount - 1);
                break;

            case TaskType.Meeting:
                meetingCount =
                    Mathf.Max(0, meetingCount - 1);
                break;
        }
    }


    private void UpdateNotificationBadges()
    {
        UpdateBadge(
            emailBadge,
            emailCount
        );

        UpdateBadge(
            callBadge,
            callCount
        );

        UpdateBadge(
            printBadge,
            printCount
        );

        UpdateBadge(
            meetingBadge,
            meetingCount
        );
    }


    private void UpdateBadge(
        NotificationBadge badge,
        int count)
    {
        if (badge == null ||
            badge.badgeObject == null)
        {
            return;
        }

        if (badge.countText != null)
        {
            badge.countText.text =
                count.ToString();
        }

        badge.badgeObject.SetActive(
            count > 0
        );
    }


    public List<RuntimeTask> GetActiveTasks()
    {
        return activeTasks;
    }


    public List<RuntimeTask> GetCompletedTasks()
    {
        return completedTasks;
    }


    public List<RuntimeTask> GetFailedTasks()
    {
        return failedTasks;
    }
}