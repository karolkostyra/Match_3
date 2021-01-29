using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, IBoardView
{
    public static BoardView Instance { get; private set; }

    public event EventHandler<CreateBoardEventArgs> OnButtonClicked;
    public event EventHandler<SwapTilesEventArgs> OnSwapTiles;
    public Vector2Int StartingBoardPosition { get => this.startingBoardPosition; set => this.startingBoardPosition = value; }
    public GameObject TilePrefab { get => this.tilePrefab; set => this.tilePrefab = value; }

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] protected Vector2Int startingBoardPosition;
    [SerializeField] protected GameObject tilePrefab;

    private GameObject tile_1;
    private GameObject tile_2;

    private void Awake()
    {
        Instance = this;
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }

    public void StartButton()
    {
        OnButtonClicked(this, new CreateBoardEventArgs(this.startingBoardPosition, this.tilePrefab));
    }

    public void RestartButton()
    {
        OnButtonClicked(this, new CreateBoardEventArgs(this.startingBoardPosition, this.tilePrefab));
    }

    public void GetSelectedTile(GameObject tile)
    {
        if (tile_1 == null)
        {
            tile_1 = tile;
            Select(tile_1);
        }
        else if (tile_1 != tile)
        {
            tile_2 = tile;
            if (Vector3.Distance(tile_1.transform.position, tile_2.transform.position) == 1)
            {
                Deselect(tile_1);
                OnSwapTiles(this, new SwapTilesEventArgs(tile_1.transform.position, tile_2.transform.position));
                ClearSelectedTiles();
            }
        }
        else
        {
            Deselect(tile_1);
            ClearSelectedTiles();
        }
    }

    public void ClearSelectedTiles()
    {
        tile_1 = tile_2 = null;
    }

    private void Select(GameObject tile)
    {
        tile.GetComponent<Tile>().Select();
    }

    private void Deselect(GameObject tile)
    {
        tile.GetComponent<Tile>().Deselect();
    }
}