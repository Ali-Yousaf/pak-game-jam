using UnityEngine;
using TMPro;
using System;

public class TaskBarClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    [Header("Task System")]
    [SerializeField] private TaskManager taskManager;

    [Header("Game Time")]
    [SerializeField] private int startHour = 9;
    [SerializeField] private int startMinute = 0;

    [Header("Game Date")]
    [SerializeField] private int startDay = 11;
    [SerializeField] private int startMonth = 8;
    [SerializeField] private int startYear = 2026;

    [Header("Speed")]
    [SerializeField] private float realSecondsPerGameMinute = 1f;

    private float timer;

    private int currentHour;
    private int currentMinute;

    private int currentDay;
    private int currentMonth;
    private int currentYear;


    private void Start()
    {
        currentHour = startHour;
        currentMinute = startMinute;

        currentDay = startDay;
        currentMonth = startMonth;
        currentYear = startYear;

        UpdateTimeText();

        // Send initial time to TaskManager.
        NotifyTaskManager();
    }


    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= realSecondsPerGameMinute)
        {
            timer -= realSecondsPerGameMinute;

            currentMinute++;

            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour++;
            }

            // Move to the next day after 11:59 PM.
            if (currentHour >= 24)
            {
                currentHour = 0;
                AdvanceDate();
            }

            UpdateTimeText();

            // Tell TaskManager that the time changed.
            NotifyTaskManager();
        }
    }


    public void SetCurrentTime(int hour, int minute)
    {
        currentHour = hour;
        currentMinute = minute;

        UpdateTimeText();

        NotifyTaskManager();
    }


    private void NotifyTaskManager()
    {
        if (taskManager == null)
            return;

        taskManager.SetCurrentTime(
            currentHour,
            currentMinute
        );
    }


    public int GetCurrentHour()
    {
        return currentHour;
    }


    public int GetCurrentMinute()
    {
        return currentMinute;
    }


    private void AdvanceDate()
    {
        DateTime date = new DateTime(
            currentYear,
            currentMonth,
            currentDay
        );

        date = date.AddDays(1);

        currentYear = date.Year;
        currentMonth = date.Month;
        currentDay = date.Day;
    }


    private void UpdateTimeText()
    {
        DateTime time = new DateTime(
            currentYear,
            currentMonth,
            currentDay,
            currentHour,
            currentMinute,
            0
        );

        timeText.text = time.ToString("h:mm tt");
        dateText.text = time.ToString("M/d/yyyy");
    }
}