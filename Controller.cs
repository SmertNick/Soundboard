using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField, Range(30, 120)] private int frameRate = 120;
    [SerializeField, Range(0, 2)] private int vSyncCount = 1;
    
    private void Awake()
    {
        QualitySettings.vSyncCount = vSyncCount;
        Application.targetFrameRate = frameRate;
    }
}