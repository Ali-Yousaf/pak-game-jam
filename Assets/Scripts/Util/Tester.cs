using UnityEngine;

public class Tester : MonoBehaviour
{
    [Header("Task System")]
    public bool enableTaskSpawning = true;
    public bool enableTaskNotifications = true;
    public bool enableTaskDeadlines = true;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    [Header("Time")]
    public bool enableGameClock = true;


    public bool IsTaskSpawningEnabled()
    {
        return enableTaskSpawning;
    }


    public bool IsTaskNotificationsEnabled()
    {
        return enableTaskNotifications;
    }


    public bool IsTaskDeadlinesEnabled()
    {
        return enableTaskDeadlines;
    }


    public bool IsDebugLogsEnabled()
    {
        return enableDebugLogs;
    }


    public bool IsGameClockEnabled()
    {
        return enableGameClock;
    }
}