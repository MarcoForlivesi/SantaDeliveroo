using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    WaitToStart,
    Playing,
    Win,
    Lost
}

public enum GiftType
{
    SnowWhite,
    StrippedGreen,
    DotWhite,
    DotRed,
    SnowBlue
}

public enum SelectableType
{
    None,
    Santa,
    House,
    Gift
}