using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MailItem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI subjectText;
    [SerializeField] private TextMeshProUGUI previewText;
    [SerializeField] private GameObject unreadIndicator;

    private RuntimeTask runtimeTask;
    private DinoMail dinoMail;


    public void Setup(RuntimeTask task, DinoMail mail)
    {
        if (task == null)
            return;

        runtimeTask = task;
        dinoMail = mail;

        TaskData data = task.taskData;

        if (senderText != null)
        {
            senderText.text = data.senderName;
        }

        if (subjectText != null)
        {
            subjectText.text = data.subject;
        }

        if (previewText != null)
        {
            previewText.text = data.mailBody;
        }

        if (unreadIndicator != null)
        {
            unreadIndicator.SetActive(true);
        }
    }


    public void OpenMail()
    {
        if (runtimeTask == null)
            return;

        if (dinoMail == null)
            return;

        if (unreadIndicator != null)
        {
            unreadIndicator.SetActive(false);
        }

        dinoMail.OpenMail(runtimeTask);
    }
}