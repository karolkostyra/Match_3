using System;
using UnityEngine;

public class SwapTilesEventArgs : EventArgs
{
    public Vector3 firstTilePosition { get; private set; }
    public Vector3 secondTilePosition { get; private set; }

    public SwapTilesEventArgs(Vector3 _firstTilePos, Vector3 _secondTilePos)
    {
        firstTilePosition = _firstTilePos;
        secondTilePosition = _secondTilePos;
    }
}
