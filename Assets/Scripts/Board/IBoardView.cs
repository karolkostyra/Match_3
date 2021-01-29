using System;
using UnityEngine;

public interface IBoardView
{
    event EventHandler OnSpaceClicked;
    event EventHandler<ButtonClickedEventArgs> OnButtonClicked;
    Vector2Int StartingBoardPosition { get; set; }
    GameObject TilePrefab { get; set; }
}
