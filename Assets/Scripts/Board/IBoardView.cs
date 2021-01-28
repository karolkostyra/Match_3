using System;
using UnityEngine;

public interface IBoardView
{
    event EventHandler OnSpaceClicked;
    event EventHandler<int> OnEclicked;
}
