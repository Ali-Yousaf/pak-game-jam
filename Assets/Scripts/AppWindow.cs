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

    private DesktopManager desktopManager;
    private AppData appData;

    private RectTransform rectTransform;

    private Vector2 originalPosition;
    private Vector2 originalSize;

    private bool isMaximized = false;
    private bool isMinimized = false;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Find the DesktopManager automatically.
        desktopManager = FindFirstObjectByType<DesktopManager>();

        // Store the original window size and position.
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;

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
            maximizeButton.onClick.AddListener(MaximizeWindow);
        }
    }


    public void Initialize(AppData data)
    {
        appData = data;
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


    private void MinimizeWindow()
    {
        if (isMinimized)
            return;

        isMinimized = true;

        rectTransform.DOKill();

        // Shrink and fade the window.
        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            rectTransform
                .DOScale(0.8f, animationDuration)
                .SetEase(Ease.InBack)
        );

        CanvasGroup canvasGroup = GetCanvasGroup();

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

        CanvasGroup canvasGroup = GetCanvasGroup();

        rectTransform.localScale = Vector3.one * 0.8f;
        canvasGroup.alpha = 0f;

        rectTransform.DOKill();
        canvasGroup.DOKill();

        rectTransform
            .DOScale(Vector3.one, animationDuration)
            .SetEase(Ease.OutBack);

        canvasGroup
            .DOFade(1f, animationDuration);
    }


    private void MaximizeWindow()
    {
        if (isMaximized)
        {
            RestoreFromMaximize();
            return;
        }

        isMaximized = true;

        rectTransform.DOKill();

        // Save current position and size.
        originalPosition = rectTransform.anchoredPosition;
        originalSize = rectTransform.sizeDelta;

        // Stretch to fill the parent.
        rectTransform
            .DOAnchorPos(Vector2.zero, animationDuration)
            .SetEase(Ease.OutQuad);

        rectTransform
            .DOSizeDelta(Vector2.zero, animationDuration)
            .SetEase(Ease.OutQuad);
    }


    private void RestoreFromMaximize()
    {
        isMaximized = false;

        rectTransform.DOKill();

        rectTransform
            .DOAnchorPos(originalPosition, animationDuration)
            .SetEase(Ease.OutQuad);

        rectTransform
            .DOSizeDelta(originalSize, animationDuration)
            .SetEase(Ease.OutQuad);
    }


    private CanvasGroup GetCanvasGroup()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        return canvasGroup;
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
            maximizeButton.onClick.RemoveListener(MaximizeWindow);
        }
    }
}