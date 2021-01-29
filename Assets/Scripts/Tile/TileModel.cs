using System;
using UnityEngine;

[Serializable]
public class TileModel : ITileModel
{
    public float ScaleX { get => this.scaleX; set => this.scaleX = value; }
    public float ScaleY { get => this.scaleY; set => this.scaleY = value; }

    [SerializeField] protected float scaleX;
    [SerializeField] protected float scaleY;
}
