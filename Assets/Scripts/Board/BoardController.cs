using System.Collections.Generic;
using UnityEngine;

public class BoardController : Board
{
    [SerializeField] private BoardModel boardModel;
    [SerializeField] private BoardView boardView;

    private GameObject boardHandler;
    private GameObject[,] grid;

    public void Awake()
    {
        SetSeed(boardModel.Seed);
        boardView.OnButtonClicked += CreateBoard;
        boardView.OnSwapTiles += SwapTiles;
    }

    private void SwapTiles(object sender, SwapTilesEventArgs e)
    {
        int xOffset = boardView.StartingBoardPosition.x;
        int yOffset = boardView.StartingBoardPosition.y;

        int firstX = (int)e.firstTilePosition.x - xOffset;
        int firstY = (int)e.firstTilePosition.y - yOffset;

        int secondX = (int)e.secondTilePosition.x - xOffset;
        int secondY = (int)e.secondTilePosition.y - yOffset;

        GameObject firstTile = grid[firstX, firstY];
        GameObject secondTile = grid[secondX, secondY];
        SpriteRenderer firstTileRenderer = firstTile.GetComponent<SpriteRenderer>();
        SpriteRenderer secondTileRenderer = secondTile.GetComponent<SpriteRenderer>();
        
        var temp = firstTile.transform.position;
        firstTile.transform.position = secondTile.transform.position;
        secondTile.transform.position = temp;

        var temp2 = grid[firstX, firstY];
        grid[firstX, firstY] = grid[secondX, secondY];
        grid[secondX, secondY] = temp2;
    }
    
    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    private void CreateBoard(object sender, CreateBoardEventArgs e)
    {
        CheckBoardHandler();
        grid = new GameObject[boardModel.Width, boardModel.Height];
        Vector2 startingPos = e.startingBoardPos;
        GameObject tile = e.tilePrefab;

        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(x+startingPos.x, y+startingPos.y, 0f),
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
            boardView.ClearSelectedTiles();
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

        if (currentX > 1) { colorList.Remove(grid[currentX - 2, currentY].GetComponent<SpriteRenderer>().color); }
        if (currentY > 1) { colorList.Remove(grid[currentX, currentY - 2].GetComponent<SpriteRenderer>().color); }
        
        int randomColor = UnityEngine.Random.Range(0, colorList.Count);
        return colorList[randomColor];
    }

    private void OnDestroy()
    {
        boardView.OnButtonClicked -= CreateBoard;
        boardView.OnSwapTiles -= SwapTiles;
    }
}