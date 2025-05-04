using UnityEngine;
using UnityEngine.UI;

public class FilterButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Binder binder;
    [SerializeField] private EntryFlags flag;
    [SerializeField] private FilterMode mode = FilterMode.Any;

    private void OnEnable()
    {
        if (button && binder)
            button.onClick.AddListener(SetFilter);
    }
    
    private void OnDisable()
    {
        if (button)
            button.onClick.RemoveListener(SetFilter);
    }

    private void SetFilter()
    {
        if (binder)
            binder.Filter(flag, mode);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        button = GetComponent<Button>();
        binder = FindFirstObjectByType<Binder>();
    }
#endif
}