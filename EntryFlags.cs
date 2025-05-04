using System;

[Flags]
public enum EntryFlags
{
    None = 0,
    Spell = 1 << 1,
    Battle = 1 << 2,
    Map = 1 << 3,
    Game = 1 << 4,
    Town = 1 << 5,
    MapSpell = 1 << 6,
    BattleSpell = 1 << 7,
    Fire = 1 << 8,
    Water = 1 << 9,
    Earth = 1 << 10,
    Air = 1 << 11,
    Rank1 = 1 << 12,
    Rank2 = 1 << 13,
    Rank3 = 1 << 14,
    Rank4 = 1 << 15,
    Rank5 = 1 << 16,
}