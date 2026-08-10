using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DesktopManager : MonoBehaviour
{
    public static DesktopManager Instance;

    [Header("Containers")]
    [SerializeField] private Transform windowContainer;
    [SerializeField] private Transform taskbarContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject taskbarButtonPrefab;

    private Dictionary<AppData, GameObject> openWindows =
        new Dictionary<AppData, GameObject>();

    private Dictionary<AppData, GameObject> taskbarButtons =
        new Dictionary<AppData, GameObject>();

    void Awake()
    {
        if(Instance == null)
            Instance = this;

        else
            Destroy(gameObject);
    }

    public void OpenApp(AppData appData)
    {
        if (appData == null)
        {
            Debug.LogWarning("AppData is null.");
            return;
        }

        // If the app is already open, bring it to the front.
        if (openWindows.ContainsKey(appData))
        {
            GameObject existingWindow = openWindows[appData];

            if (existingWindow != null)
            {
                BringToFront(existingWindow);
            }

            return;
        }

        // Create the app window.
        GameObject window = Instantiate(
            appData.windowPrefab,
            windowContainer
        );

        openWindows.Add(appData, window);

        // Create the taskbar button.
        CreateTaskbarButton(appData, window);

        // Bring the window to the front.
        BringToFront(window);

        // Play opening animation.
        AnimateWindowOpen(window);
    }


    private void CreateTaskbarButton(AppData appData, GameObject window)
    {
        if (taskbarButtons.ContainsKey(appData))
            return;

        GameObject buttonObject = Instantiate(
            taskbarButtonPrefab,
            taskbarContainer
        );

        taskbarButtons.Add(appData, buttonObject);

        // Set taskbar icon.
        Image icon = buttonObject.GetComponentInChildren<Image>();

        if (icon != null)
        {
            icon.sprite = appData.appIcon;
        }

        // Make taskbar button focus the window.
        Button button = buttonObject.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (window != null)
                {
                    BringToFront(window);
                }
            });
        }
    }


    public void CloseApp(AppData appData)
    {
        if (appData == null)
            return;

        // Close window.
        if (openWindows.ContainsKey(appData))
        {
            GameObject window = openWindows[appData];

            if (window != null)
            {
                Destroy(window);
            }

            openWindows.Remove(appData);
        }

        // Remove taskbar button.
        if (taskbarButtons.ContainsKey(appData))
        {
            GameObject taskbarButton = taskbarButtons[appData];

            if (taskbarButton != null)
            {
                Destroy(taskbarButton);
            }

            taskbarButtons.Remove(appData);
        }
    }


    private void BringToFront(GameObject window)
    {
        if (window == null)
            return;

        window.transform.SetAsLastSibling();
    }


    private void AnimateWindowOpen(GameObject window)
    {
        RectTransform rect = window.GetComponent<RectTransform>();

        if (rect == null)
            return;

        // Start slightly smaller.
        rect.localScale = Vector3.one * 0.85f;

        // Get or create CanvasGroup.
        CanvasGroup canvasGroup =
            window.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = window.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;

        // Kill any previous tweens.
        rect.DOKill();
        canvasGroup.DOKill();

        // Scale animation.
        rect.DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.OutBack);

        // Fade animation.
        canvasGroup.DOFade(1f, 0.15f);
    }
}