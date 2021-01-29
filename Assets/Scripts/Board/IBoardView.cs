using System;
using UnityEngine;

public interface IBoardView
{
    event EventHandler<CreateBoardEventArgs> OnButtonClicked;
    event EventHandler<SwapTilesEventArgs> OnSwapTiles;
    Vector2Int StartingBoardPosition { get; set; }
    GameObject TilePrefab { get; set; }
}
