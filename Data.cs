using System;
using System.Collections.Generic;
using UnityEngine;

public class Data : ScriptableObject
{
    [SerializeField] private SoundButton prefab;
    [SerializeField] private Entry[] entries = Array.Empty<Entry>();
    public IReadOnlyList<Entry> Entries => entries;
    public SoundButton Prefab => prefab;
}