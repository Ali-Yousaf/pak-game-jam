using UnityEngine;

[CreateAssetMenu(fileName = "NewAppData", menuName = "Dino Office/App Data")]
public class AppData : ScriptableObject
{
    public string appName;
    public Sprite appIcon;
    public GameObject windowPrefab;
}