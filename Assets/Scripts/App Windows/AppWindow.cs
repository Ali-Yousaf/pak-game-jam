using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AppWindow : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button minimizeButton;
    [SerializeField] private Button maximizeButton;

    [Header("Window Settings")]
    [SerializeField] private float animationDuration = 0.2f;

    [Header("Maximized Size")]
    [SerializeField] private float maximizedWidth = 1500f;
    [SerializeField] private float maximizedHeight = 750f;

    private DesktopManager desktopManager;
    private AppData appData;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 originalPosition;
    private Vector2 originalSize;

    private Vector2 originalAnchorMin;
    private Vector2 originalAnchorMax;
    private Vector2 originalPivot;

    private bool isMaximized = false;
    private bool isMinimized = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        desktopManager = FindFirstObjectByType<DesktopManager>();

        // Store the original window settings.
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;

        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;

        // Connect buttons.
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseWindow);
        }

        if (minimizeButton != null)
        {
            minimizeButton.onClick.AddListener(MinimizeWindow);
        }

        if (maximizeButton != null)
        {
            maximizeButton.onClick.AddListener(ToggleMaximize);
        }
    }


    public void Initialize(AppData data)
    {
        appData = data;
    }


    public bool IsMinimized()
    {
        return isMinimized;
    }


    public bool IsMaximized()
    {
        return isMaximized;
    }


    private void CloseWindow()
    {
        if (desktopManager != null && appData != null)
        {
            desktopManager.CloseApp(appData);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void MinimizeWindow()
    {
        if (isMinimized)
            return;

        isMinimized = true;

        rectTransform.DOKill();
        canvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            rectTransform
                .DOScale(0.8f, animationDuration)
                .SetEase(Ease.InBack)
        );

        sequence.Join(
            canvasGroup
                .DOFade(0f, animationDuration)
        );

        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }


    public void RestoreWindow()
    {
        if (!isMinimized)
            return;

        isMinimized = false;

        gameObject.SetActive(true);

        rectTransform.DOKill();
        canvasGroup.DOKill();

        rectTransform.localScale = Vector3.one * 0.8f;
        canvasGroup.alpha = 0f;

        rectTransform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);

        canvasGroup
            .DOFade(1f, animationDuration);
    }


    private void ToggleMaximize()
    {
        if (isMaximized)
        {
            RestoreFromMaximize();
        }
        else
        {
            MaximizeWindow();
        }
    }


    private void MaximizeWindow()
    {
        if (isMaximized)
            return;

        isMaximized = true;

        rectTransform.DOKill();

        // Store current settings before maximizing.
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;

        originalAnchorMin = rectTransform.anchorMin;
        originalAnchorMax = rectTransform.anchorMax;
        originalPivot = rectTransform.pivot;

        // Center the pivot.
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Center the anchors.
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

        // Animate to center.
        rectTransform
            .DOAnchorPos(Vector2.zero, animationDuration)
            .SetEase(Ease.OutQuad);

        // Animate to 1400 x 750.
        rectTransform
            .DOSizeDelta(
                new Vector2(
                    maximizedWidth,
                    maximizedHeight
                ),
                animationDuration
            )
            .SetEase(Ease.OutQuad);
    }


    private void RestoreFromMaximize()
    {
        if (!isMaximized)
            return;

        isMaximized = false;

        rectTransform.DOKill();

        // Restore anchors and pivot.
        rectTransform.anchorMin = originalAnchorMin;
        rectTransform.anchorMax = originalAnchorMax;
        rectTransform.pivot = originalPivot;

        // Restore original position.
        rectTransform
            .DOAnchorPos(
                originalPosition,
                animationDuration
            )
            .SetEase(Ease.OutQuad);

        // Restore original size.
        rectTransform
            .DOSizeDelta(
                originalSize,
                animationDuration
            )
            .SetEase(Ease.OutQuad);
    }


    public void FocusWindow()
    {
        if (isMinimized)
        {
            RestoreWindow();
        }

        if (desktopManager != null)
        {
            desktopManager.BringWindowToFront(gameObject);
        }
    }


    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(CloseWindow);
        }

        if (minimizeButton != null)
        {
            minimizeButton.onClick.RemoveListener(MinimizeWindow);
        }

        if (maximizeButton != null)
        {
            maximizeButton.onClick.RemoveListener(ToggleMaximize);
        }
    }
}