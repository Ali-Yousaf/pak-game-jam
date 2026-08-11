using UnityEngine;

public enum TaskType
{
    Email,
    Call,
    Print,
    Meeting
}

[CreateAssetMenu(
    fileName = "NewTask",
    menuName = "Dino Office/Task Data"
)]
public class TaskData : ScriptableObject
{
    [Header("Basic Information")]
    public string taskTitle;
    [TextArea(2, 5)]
    public string description;

    public TaskType taskType;

    [Header("Timing")]
    public int deadlineHour;
    public int deadlineMinute;

    [Header("Task Effects")]
    public int productivityReward = 10;
    public int productivityPenalty = 10;

    [Header("Task Settings")]
    public bool isUrgent;
}