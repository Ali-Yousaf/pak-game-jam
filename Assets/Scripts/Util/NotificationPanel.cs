using UnityEngine;
using TMPro;
using DG.Tweening;

public class NotificationPanel : MonoBehaviour
{
    public static NotificationPanel Instance;

    [Header("Panel")]
    [SerializeField] private RectTransform panel;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI taskTypeText;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private float displayDuration = 3f;

    [SerializeField] private Vector2 hiddenPosition =
        new Vector2(400f, 0f);

    [SerializeField] private Vector2 shownPosition =
        new Vector2(0f, 0f);

    private Sequence notificationSequence;


    private void Awake()
    {
        if(Instance == null)
            Instance = this;

        else
            Destroy(gameObject);

        panel.anchoredPosition = hiddenPosition;
        panel.gameObject.SetActive(false);
    }


    public void ShowNotificationPanel(
        string title,
        string description,
        string taskType)
    {
        notificationSequence?.Kill();

        titleText.text = title;
        descriptionText.text = description;
        taskTypeText.text = taskType;

        panel.gameObject.SetActive(true);

        notificationSequence = DOTween.Sequence();

        notificationSequence.Append(
            panel.DOAnchorPos(
                shownPosition,
                animationDuration
            ).SetEase(Ease.OutBack)
        );

        notificationSequence.AppendInterval(
            displayDuration
        );

        notificationSequence.Append(
            panel.DOAnchorPos(
                hiddenPosition,
                animationDuration
            ).SetEase(Ease.InBack)
        );

        notificationSequence.OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
        });
    }


    public void ShowNotificationPanel(TaskData task)
    {
        if (task == null)
            return;

        ShowNotificationPanel(
            task.taskTitle,
            task.description,
            task.taskType.ToString()
        );
    }
}