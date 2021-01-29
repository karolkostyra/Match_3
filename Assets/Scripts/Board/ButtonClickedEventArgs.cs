using System;
using UnityEngine;

public class ButtonClickedEventArgs : EventArgs
{
    public Vector2Int startingBoardPos { get; private set; }
    public GameObject tilePrefab { get; private set; }

    public ButtonClickedEventArgs(Vector2Int startingPos, GameObject tile)
    {
        startingBoardPos = startingPos;
        tilePrefab = tile;
    }
}
