using System;
using UnityEngine;

public class ButtonClickedEventArgs : EventArgs
{
    public Vector3 startingBoardPos { get; private set; }
    public GameObject tilePrefab { get; private set; }

    public ButtonClickedEventArgs(Vector3 startingPos, GameObject tile)
    {
        startingBoardPos = startingPos;
        tilePrefab = tile;
    }
}
