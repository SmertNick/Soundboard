using UnityEngine;

[CreateAssetMenu(fileName = "New Entry", menuName = "Soundboard/Entry", order = 0)]
public class Entry : ScriptableObject
{
    [field: SerializeField] public Sprite Icon;
    [field: SerializeField] public AudioClip Sound;
    [field: SerializeField] public EntryFlags Flags;
}