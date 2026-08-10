using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DesktopManager : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private Transform windowContainer;
    [SerializeField] private Transform taskbarContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject taskbarButtonPrefab;

    private Dictionary<AppData, GameObject> openWindows =
        new Dictionary<AppData, GameObject>();

    private Dictionary<AppData, GameObject> taskbarButtons =
        new Dictionary<AppData, GameObject>();


    public void OpenApp(AppData appData)
    {
        if (appData == null)
        {
            Debug.LogWarning("AppData is null.");
            return;
        }

        // If the app is already open.
        if (openWindows.ContainsKey(appData))
        {
            GameObject existingWindow = openWindows[appData];

            if (existingWindow != null)
            {
                AppWindow appWindow =
                    existingWindow.GetComponent<AppWindow>();

                // Restore if minimized.
                if (appWindow != null)
                {
                    appWindow.RestoreWindow();
                }

                // Bring the window to the front.
                BringToFront(existingWindow);
            }

            return;
        }

        // Create the app window.
        GameObject window = Instantiate(
            appData.windowPrefab,
            windowContainer
        );

        // Initialize AppWindow.
        AppWindow appWindowComponent =
            window.GetComponent<AppWindow>();

        if (appWindowComponent != null)
        {
            appWindowComponent.Initialize(appData);
        }

        openWindows.Add(appData, window);

        // Create taskbar button.
        CreateTaskbarButton(appData, window);

        // Bring window to front.
        BringToFront(window);

        // Play opening animation.
        AnimateWindowOpen(window);
    }


    private void CreateTaskbarButton(
        AppData appData,
        GameObject window)
    {
        if (taskbarButtons.ContainsKey(appData))
            return;

        GameObject buttonObject = Instantiate(
            taskbarButtonPrefab,
            taskbarContainer
        );

        taskbarButtons.Add(appData, buttonObject);

        // Set the taskbar icon.
        Image icon =
            buttonObject.GetComponentInChildren<Image>();

        if (icon != null)
        {
            icon.sprite = appData.appIcon;
        }

        // Taskbar button functionality.
        Button button =
            buttonObject.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (window == null)
                    return;

                AppWindow appWindow =
                    window.GetComponent<AppWindow>();

                // Restore if minimized.
                if (appWindow != null)
                {
                    appWindow.RestoreWindow();
                }

                // Bring to front.
                BringToFront(window);
            });
        }
    }


    public void CloseApp(AppData appData)
    {
        if (appData == null)
            return;

        // Destroy the window.
        if (openWindows.ContainsKey(appData))
        {
            GameObject window =
                openWindows[appData];

            if (window != null)
            {
                Destroy(window);
            }

            openWindows.Remove(appData);
        }

        // Destroy the taskbar button.
        if (taskbarButtons.ContainsKey(appData))
        {
            GameObject taskbarButton =
                taskbarButtons[appData];

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
        if (window == null)
            return;

        RectTransform rect =
            window.GetComponent<RectTransform>();

        if (rect == null)
            return;

        CanvasGroup canvasGroup =
            window.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup =
                window.AddComponent<CanvasGroup>();
        }

        // Starting state.
        rect.localScale = Vector3.one * 0.85f;
        canvasGroup.alpha = 0f;

        // Kill any existing tweens.
        rect.DOKill();
        canvasGroup.DOKill();

        // Scale animation.
        rect.DOScale(
            Vector3.one,
            0.2f
        )
        .SetEase(Ease.OutBack);

        // Fade animation.
        canvasGroup.DOFade(
            1f,
            0.15f
        );
    }
}