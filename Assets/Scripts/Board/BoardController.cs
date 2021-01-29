using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : Board
{
    public static BoardController Instance { get; private set; }
    
    [SerializeField] private BoardModel boardModel;
    [SerializeField] private BoardView boardView;

    private GameObject boardHandler;
    private GameObject[,] grid;

    private GameObject tile_1;
    private GameObject tile_2;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        SetSeed(boardModel.Seed);
       // boardView.OnSpaceClicked += CreateBoard;
        boardView.OnButtonClicked += CreateBoard;
    }

    public void GetTile(GameObject tile)
    {
        if(tile_1 == null)
        {
            tile_1 = tile;
            Select(tile_1);
            
        }
        else if(tile_1 != tile && Vector3.Distance(tile_1.transform.position, tile.transform.position) == 1)
        {
            tile_2 = tile;
            Deselect(tile_1);
            SwapTiles(tile_1.transform.position, tile_2.transform.position);
            tile_1 = tile_2 = null;
        }
    }

    private void Select(GameObject tile)
    {
        SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
        Color32 temp = tileRenderer.color;
        tileRenderer.color = new Color32(temp.r, temp.g, temp.b, 150);
    }

    private void Deselect(GameObject tile)
    {
        SpriteRenderer tileRenderer = tile.GetComponent<SpriteRenderer>();
        Color32 temp = tileRenderer.color;
        tileRenderer.color = new Color32(temp.r, temp.g, temp.b, 255);
    }

    private void SwapTiles(Vector3 firstTilePos, Vector3 secondTilePos)
    {
        GameObject firstTile = grid[(int)firstTilePos.x, (int)firstTilePos.y];
        SpriteRenderer firstTileRenderer = firstTile.GetComponent<SpriteRenderer>();

        GameObject secondTile = grid[(int)secondTilePos.x, (int)secondTilePos.y];
        SpriteRenderer secondTileRenderer = secondTile.GetComponent<SpriteRenderer>();

        Color32 temp = firstTileRenderer.color;
        if(temp != secondTileRenderer.color)
        {
            firstTileRenderer.color = secondTileRenderer.color;
            secondTileRenderer.color = temp;
            Debug.Log(firstTile.name + " - " + secondTile.name);
        }
        else
        {
            Debug.Log("same color");
        }
    }
    
    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    private void CreateBoard(object sender, ButtonClickedEventArgs e)
    {
        CheckBoardHandler();
        
        grid = new GameObject[boardModel.Width, boardModel.Height];
        Vector2 startingPos = e.startingBoardPos;
        GameObject tile = e.tilePrefab;

        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                GameObject newTile = Instantiate(tile, GetNextPosition(new Vector3(x+startingPos.x, y+startingPos.y, 0f)),
                                                 tile.transform.rotation, boardHandler.transform);
                
                newTile.GetComponent<SpriteRenderer>().color = GetRandomMismatchColor(x, y);
                newTile.name = x.ToString() + ' ' + y.ToString();
                grid[x, y] = newTile;
            }
        }
    }
    
    private void CheckBoardHandler()
    {
        if (boardHandler == null)
        {
            boardHandler = new GameObject("BoardHandler");
            boardHandler.transform.parent = this.gameObject.transform;
        }
        else
        {
            ClearBoard();
        }
    }

    private void ClearBoard()
    {
        foreach(var tile in grid)
        {
            Destroy(tile);
        }
    }

    private Color32 GetRandomMismatchColor(int currentX, int currentY)
    {
        List<Color32> colorList = new List<Color32>();
        colorList.AddRange(boardModel.Colors);

        if (currentX > 1)
        {
            colorList.Remove(grid[currentX - 2, currentY].GetComponent<SpriteRenderer>().color);
        }
        if (currentY > 1)
        {
            colorList.Remove(grid[currentX, currentY - 2].GetComponent<SpriteRenderer>().color);
        }
        
        int randomColor = UnityEngine.Random.Range(0, colorList.Count);
        return colorList[randomColor];
    }

    private Vector3 GetNextPosition(Vector3 position)
    {
        int size = boardModel.CellSize;
        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = 0;
        
        return new Vector3((float)xCount * size, (float)yCount * size, (float)zCount);
    }

    private void OnDestroy()
    {
        //boardView.OnSpaceClicked -= CreateBoard;
        boardView.OnButtonClicked -= CreateBoard;
    }
}