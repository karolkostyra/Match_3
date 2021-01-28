using System;
using UnityEngine;

public interface IBoardView
{
    event EventHandler OnSpaceClicked;
    event EventHandler<Vector2> OnButtonClicked;
    Vector2 StartingBoardPosition { get; set; }
}
