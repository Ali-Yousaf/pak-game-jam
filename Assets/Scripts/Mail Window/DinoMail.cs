using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DinoMail : MonoBehaviour
{
    [Header("Inbox")]
    [SerializeField] private Transform inboxContainer;
    [SerializeField] private MailItem mailItemPrefab;


    [Header("Reading Panel")]
    [SerializeField] private GameObject readingPanel;
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private TextMeshProUGUI bodyText;


    private List<MailItem> mailItems = new List<MailItem>();
    private RuntimeTask currentMail;


    private void Start()
    {
        if (readingPanel != null)
        {
            readingPanel.SetActive(false);
        }

        RefreshInbox();
    }


    public void RefreshInbox()
    {
        ClearInbox();

        if (inboxContainer == null)
        {
            Debug.LogWarning("DinoMail: Inbox Container is not assigned.");
            return;
        }

        if (mailItemPrefab == null)
        {
            Debug.LogWarning("DinoMail: Mail Item Prefab is not assigned.");
            return;
        }

        if (TaskManager.Instance == null)
        {
            Debug.LogWarning("DinoMail: TaskManager.Instance is null.");
            return;
        }

        List<RuntimeTask> tasks = TaskManager.Instance.GetActiveTasks();

        Debug.Log("DinoMail: Active tasks = " + tasks.Count);

        foreach (RuntimeTask task in tasks)
        {
            if (task == null)
                continue;

            if (task.taskData == null)
                continue;

            if (task.taskData.taskType != TaskType.Email)
                continue;

            Debug.Log("DinoMail: Adding email: " + task.taskData.taskTitle);

            CreateMailItem(task);
        }
    }


    private void CreateMailItem(RuntimeTask task)
    {
        MailItem item = Instantiate(mailItemPrefab, inboxContainer);

        item.Setup(task, this);

        mailItems.Add(item);
    }


    public void OpenMail(RuntimeTask task)
    {
        if (task == null)
            return;

        currentMail = task;

        TaskData data = task.taskData;

        if (readingPanel != null)
        {
            readingPanel.SetActive(true);
        }

        senderText.text = data.senderName;
        emailText.text = data.senderEmail;
        subjectText.text = data.subject;
        bodyText.text = data.mailBody;
    }


    public void CompleteCurrentMail()
    {
        if (currentMail == null)
            return;

        if (TaskManager.Instance == null)
            return;

        TaskManager.Instance.CompleteTask(currentMail);

        currentMail = null;

        CloseMail();
        RefreshInbox();
    }


    public void CloseMail()
    {
        if (readingPanel != null)
        {
            readingPanel.SetActive(false);
        }

        currentMail = null;
    }


    private void ClearInbox()
    {
        foreach (MailItem item in mailItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        mailItems.Clear();
    }
}