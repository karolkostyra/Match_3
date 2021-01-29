using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour, ITileController
{
    [SerializeField] private TileView tileView;
    [SerializeField] private TileModel tileModel;

    void Start()
    {
    }
}
