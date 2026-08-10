using UnityEngine;
using UnityEngine.UI;

public class AppWindow : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button minimizeButton;
    [SerializeField] private Button maximizeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        minimizeButton.onClick.AddListener(Minimize);
        maximizeButton.onClick.AddListener(Maximize);
    }

    private void Close()
    {
        
    }

    private void Minimize()
    {
        gameObject.SetActive(false);
    }

    private void Maximize()
    {
        // Change window size here
    }
}