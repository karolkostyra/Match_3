using System;
using System.Collections;
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
    [SerializeField] private float swapSpeed = 3f;
    [SerializeField] protected Vector2Int startingBoardPosition;
    [SerializeField] protected GameObject tilePrefab;

    private GameObject tile_1;
    private GameObject tile_2;
    private bool isMoving = false;

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
        if (isMoving)
        {
            return;
        }
        if (tile_1 == null)
        {
            tile_1 = tile;
            Select(tile_1);
        }
        else if (tile_1 != tile)
        {
            isMoving = true;
            tile_2 = tile;
            if (Vector3.Distance(tile_1.transform.position, tile_2.transform.position) == 1)
            {
                Deselect(tile_1);
                StartCoroutine(SwapTiles(tile_1.transform.position, tile_2.transform.position));
            }
        }
        else
        {
            isMoving = false;
            Deselect(tile_1);
            ClearSelectedTiles();
        }
    }

    IEnumerator SwapTiles(Vector3 t1, Vector3 t2)
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * swapSpeed;
            tile_1.transform.position = Vector3.Lerp(tile_1.transform.position, t2, i);
            tile_2.transform.position = Vector3.Lerp(tile_2.transform.position, t1, i);
            yield return 0;
        }
        OnSwapTiles(this, new SwapTilesEventArgs(tile_1.transform.position, tile_2.transform.position));
        ClearSelectedTiles();
        isMoving = false;
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