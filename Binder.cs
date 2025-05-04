using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Binder : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup view;
    [SerializeField] private Data data;
    [SerializeField] private AudioSource audioSource;
    
    private List<SoundButton> _buttons;

    private void Awake()
    {
        Init(data);
    }

    private void Init(Data data)
    {
        var entries = Sort(data.Entries);
        _buttons = new List<SoundButton>(entries.Count);
        var prefab = data.Prefab;
        var parent = view.transform;
        
        foreach (var entry in entries)
        {
            var button = Instantiate(prefab, parent);
            button.Init(entry, audioSource);
            _buttons.Add(button);
        }
    }

    public void Filter(EntryFlags flag, FilterMode mode)
    {
        foreach (var button in _buttons)
        {
            var mask = button.Flags & flag;
            var isActive = mode == FilterMode.Any && mask != 0 || mode == FilterMode.All && mask == flag;
            button.gameObject.SetActive(isActive);
        }
    }

    private static List<Entry> Sort(IReadOnlyList<Entry> entries)
    {
        var count = entries.Count;
        var all = new List<Entry>(count);
        var ranks = new List<List<Entry>>(6);
        for (var i = 0; i < 6; i++)
            ranks.Add(new List<Entry>(count));

        for (var i = 0; i < count; i++)
        {
            var entry = entries[i];
            var flags = entry.Flags;

            if ((flags & EntryFlags.Rank1) == EntryFlags.Rank1)
                ranks[1].Add(entry);
            else if ((flags & EntryFlags.Rank2) == EntryFlags.Rank2)
                ranks[2].Add(entry);
            else if ((flags & EntryFlags.Rank3) == EntryFlags.Rank3)
                ranks[3].Add(entry);
            else if ((flags & EntryFlags.Rank4) == EntryFlags.Rank4)
                ranks[4].Add(entry);
            else if ((flags & EntryFlags.Rank5) == EntryFlags.Rank5)
                ranks[5].Add(entry);
            else
                ranks[0].Add(entry);
        }

        foreach (var rank in ranks)
        {
            var list = rank;
            Sort(ref list);
            
            foreach (var entry in list.Where(entry => !all.Contains(entry)))
                all.Add(entry);
        }

        return all;
    }

    private static void Sort(ref List<Entry> list)
    {
        list.Sort((a, b) => string.Compare(a.name, b.name, StringComparison.InvariantCultureIgnoreCase));
    }
}