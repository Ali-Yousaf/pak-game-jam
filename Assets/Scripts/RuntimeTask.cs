using System;

[Serializable]
public class RuntimeTask
{
    public TaskData taskData;

    public int spawnHour;
    public int spawnMinute;

    public int deadlineHour;
    public int deadlineMinute;

    public bool isCompleted;
    public bool isFailed;


    public RuntimeTask(
        TaskData data,
        int spawnHour,
        int spawnMinute)
    {
        taskData = data;

        this.spawnHour = spawnHour;
        this.spawnMinute = spawnMinute;

        CalculateDeadline();
    }


    private void CalculateDeadline()
    {
        DateTime spawnTime = new DateTime(
            2026,
            1,
            1,
            spawnHour,
            spawnMinute,
            0
        );

        DateTime deadline =
            spawnTime.AddMinutes(taskData.timeLimitMinutes);

        deadlineHour = deadline.Hour;
        deadlineMinute = deadline.Minute;
    }
}