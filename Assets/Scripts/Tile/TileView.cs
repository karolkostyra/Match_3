using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour, ITileView
{
    public event EventHandler OnTileInstantiated;

    private void Start()
    {
        OnTileInstantiated(this, EventArgs.Empty);
    }
}
