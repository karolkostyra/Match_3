using System;
using UnityEngine;

public class CreateBoardEventArgs : EventArgs
{
    public Vector2Int startingBoardPos { get; private set; }
    public GameObject tilePrefab { get; private set; }

    public CreateBoardEventArgs(Vector2Int _startingBoardPos, GameObject _tilePrefab)
    {
        startingBoardPos = _startingBoardPos;
        tilePrefab = _tilePrefab;
    }
}
