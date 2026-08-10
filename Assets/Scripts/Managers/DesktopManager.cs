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

    [Header("Window Animation")]
    [SerializeField] private float openAnimationDuration = 0.2f;

    private Dictionary<AppData, GameObject> openWindows =
        new Dictionary<AppData, GameObject>();

    private Dictionary<AppData, GameObject> taskbarButtons =
        new Dictionary<AppData, GameObject>();


    // Called by an app icon on the desktop.
    public void OpenApp(AppData appData)
    {
        if (appData == null)
        {
            Debug.LogWarning("AppData is null.");
            return;
        }

        // Check if the app is already open.
        if (openWindows.ContainsKey(appData))
        {
            GameObject existingWindow = openWindows[appData];

            if (existingWindow != null)
            {
                AppWindow appWindow =
                    existingWindow.GetComponent<AppWindow>();

                // If minimized, restore it.
                if (appWindow != null &&
                    appWindow.IsMinimized())
                {
                    appWindow.RestoreWindow();
                }

                BringWindowToFront(existingWindow);
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

        // Create the taskbar button.
        CreateTaskbarButton(
            appData,
            window
        );

        // Bring window to front.
        BringWindowToFront(window);

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

        taskbarButtons.Add(
            appData,
            buttonObject
        );

        // Find the icon inside the taskbar button.
        Image icon =
            buttonObject.GetComponentInChildren<Image>();

        if (icon != null)
        {
            icon.sprite = appData.appIcon;
        }

        // Get button component.
        Button button =
            buttonObject.GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                HandleTaskbarClick(
                    appData,
                    window
                );
            });
        }
    }


    private void HandleTaskbarClick(
        AppData appData,
        GameObject window)
    {
        if (window == null)
            return;

        AppWindow appWindow =
            window.GetComponent<AppWindow>();

        if (appWindow == null)
            return;

        // If minimized → restore.
        if (appWindow.IsMinimized())
        {
            appWindow.RestoreWindow();

            BringWindowToFront(window);

            return;
        }

        // If currently visible → minimize.
        appWindow.MinimizeWindow();
    }


    public void CloseApp(AppData appData)
    {
        if (appData == null)
            return;

        // Remove window.
        if (openWindows.ContainsKey(appData))
        {
            GameObject window =
                openWindows[appData];

            if (window != null)
            {
                window.transform.DOKill();

                CanvasGroup canvasGroup =
                    window.GetComponent<CanvasGroup>();

                if (canvasGroup != null)
                {
                    canvasGroup.DOKill();
                }

                Destroy(window);
            }

            openWindows.Remove(appData);
        }

        // Remove taskbar button.
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


    public void BringWindowToFront(GameObject window)
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

        // Kill existing tweens.
        rect.DOKill();
        canvasGroup.DOKill();

        // Scale pop.
        rect
            .DOScale(
                Vector3.one,
                openAnimationDuration
            )
            .SetEase(Ease.OutBack);

        // Fade in.
        canvasGroup
            .DOFade(
                1f,
                openAnimationDuration * 0.75f
            );
    }
}