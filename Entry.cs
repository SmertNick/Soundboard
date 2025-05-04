using UnityEngine;

[CreateAssetMenu(fileName = "New Entry", menuName = "Soundboard/Entry", order = 0)]
public class Entry : ScriptableObject
{
    [SerializeField] private Sprite icon;
    [SerializeField] private AudioClip sound;
    [SerializeField] private EntryFlags flags;
    
    public Sprite Icon => icon;
    public AudioClip Sound => sound;
    public EntryFlags Flags => flags;
}