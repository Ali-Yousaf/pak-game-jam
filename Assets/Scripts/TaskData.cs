using UnityEngine;

public enum TaskType
{
    Email,
    Call,
    Print,
    Meeting
}

public enum TaskPriority
{
    Low,
    Normal,
    High,
    Urgent
}

[CreateAssetMenu(
    fileName = "NewTask",
    menuName = "Dino Office/Task Data"
)]
public class TaskData : ScriptableObject
{
    [Header("Basic Information")]
    public string taskTitle;

    [TextArea(3, 6)]
    public string description;

    public TaskType taskType;

    public TaskPriority priority = TaskPriority.Normal;

    [Header("Task Details")]
    public string senderName;

    public string department;

    [TextArea(2, 4)]
    public string additionalInformation;

    [Header("Timing")]
    [Tooltip("How long the player has to complete the task after it appears.")]
    public int timeLimitMinutes = 10;

    [Header("Rewards & Penalties")]
    public int productivityReward = 10;

    public int productivityPenalty = 10;

    public int stressReward = 0;

    public int stressPenalty = 10;

    [Header("Task Settings")]
    public bool isUrgent;

    public bool canExpire = true;
}