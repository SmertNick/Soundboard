using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;

    private AudioClip _sound;
    private AudioSource _source;
    public EntryFlags Flags { get; private set; }

    private void OnEnable() => button.onClick.AddListener(PlaySound);
    private void OnDisable() => button.onClick.RemoveListener(PlaySound);

    public void Init(Entry entry, AudioSource source)
    {
        _sound = entry.Sound;
        _source = source;
        Flags = entry.Flags;
        image.sprite = entry.Icon;
        text.text = entry.name;
    }

    private void PlaySound()
    {
        if (_sound != null)
            _source.PlayOneShot(_sound);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();
    }
#endif
}