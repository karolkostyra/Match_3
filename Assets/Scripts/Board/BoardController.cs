using System.Collections.Generic;
using UnityEngine;

public class BoardController : Board, IBoardController
{
    [SerializeField] private BoardModel boardModel;
    [SerializeField] private BoardView boardView;

    private GameObject boardHandler;
    private GameObject currentTile;
    private GameObject[,] grid, gridCopy1, gridCopy2;
    private List<GameObject> matchingTilesInRow, matchingTilesInCol;
    private bool clearedTiles;
    private GameObject tilePrefab;
    private Vector2Int startingBoardPos;

    public void Awake()
    {
        SetSeed(boardModel.Seed);
        boardView.OnButtonClicked += CreateBoard;
        boardView.OnSwapTiles += SwapTiles;
        matchingTilesInRow = new List<GameObject>();
        matchingTilesInCol = new List<GameObject>();
    }

    private void SwapTiles(object sender, SwapTilesEventArgs e)
    {
        Vector2Int firstTilePos = GetCorrectPosition(e.firstTilePosition);
        Vector2Int secondTilePos = GetCorrectPosition(e.secondTilePosition);

        var tempTile = grid[firstTilePos.x, firstTilePos.y];
        var tempTileName = tempTile.name;

        grid[firstTilePos.x, firstTilePos.y].name = grid[secondTilePos.x, secondTilePos.y].name;
        grid[secondTilePos.x, secondTilePos.y].name = tempTileName;
        grid[firstTilePos.x, firstTilePos.y] = grid[secondTilePos.x, secondTilePos.y];
        grid[secondTilePos.x, secondTilePos.y] = tempTile;

        FindMatchingTiles(grid);
    }

    private void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    private void CreateBoard(object sender, CreateBoardEventArgs e)
    {
        CheckBoardHandler();
        grid = new GameObject[boardModel.Width, boardModel.Height];
        startingBoardPos = e.startingBoardPos;
        tilePrefab = e.tilePrefab;

        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                grid[x, y] = InstantiateTile(x, y);
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
            boardView.ClearSelectedTiles();
            ClearBoard();
        }
    }

    private void ClearBoard()
    {
        foreach (var tile in grid)
        {
            Destroy(tile);
        }
    }

    private Color32 GetRandomMismatchColor(int currentX, int currentY)
    {
        List<Color32> colorList = new List<Color32>();
        int maxRange = boardModel.ColorCount;
        for (int i = 0; i < boardModel.ColorCount; i++)
        {
            colorList.Add(boardModel.Colors[i]);
        }
        if (currentX > 1) { colorList.Remove(grid[currentX - 2, currentY].GetComponent<SpriteRenderer>().color); maxRange--; }
        if (currentY > 1) { colorList.Remove(grid[currentX, currentY - 2].GetComponent<SpriteRenderer>().color); maxRange--; }
        
        int randomColor = Random.Range(0, maxRange);
        return colorList[randomColor];
    }

    private void FindMatchingTiles(GameObject[,] _grid)
    {
        CopyGrid(_grid);
        clearedTiles = false;
        currentTile = null;
        matchingTilesInRow.Clear();
        matchingTilesInCol.Clear();

        for (int y = 0; y < boardModel.Height; y++)
        {
            for (int x = 0; x < boardModel.Width; x++)
            {
                CheckTile(x, y, gridCopy1);
                if (matchingTilesInRow.Count >= 3)
                {
                    RemoveMatchingTiles(matchingTilesInRow);
                    FillEmptyCells(matchingTilesInRow);
                }
                currentTile = null;
                matchingTilesInRow.Clear();
            }
        }
        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                CheckTile(x, y, gridCopy2, false);
                if (matchingTilesInCol.Count >= 3)
                {
                    RemoveMatchingTiles(matchingTilesInCol);
                    FillEmptyCells(matchingTilesInCol);
                }
                currentTile = null;
                matchingTilesInCol.Clear();
            }
        }
        if (clearedTiles)
        {
            FindMatchingTiles(grid);
        }
    }

    private void CheckTile(int x, int y, GameObject[,] gridCopy, bool inRow = true)
    {
        if (gridCopy[x, y] == null)
        {
            return;
        }
        if (currentTile == null)
        {
            currentTile = gridCopy[x, y];
            if (inRow) { AddToMatchingTilesInRow(x, y); }
            else { AddToMatchingTilesInCol(x, y); }
            gridCopy[x, y] = null;
        }
        else if (currentTile.GetComponent<SpriteRenderer>().color != gridCopy[x, y].GetComponent<SpriteRenderer>().color)
        {
            return;
        }
        else
        {
            if (inRow) { AddToMatchingTilesInRow(x, y); }
            else { AddToMatchingTilesInCol(x, y); }
            gridCopy[x, y] = null;
        }

        if (inRow)
        {
            if (x > 0) { CheckTile(x - 1, y, gridCopy, true); }
            if (x < boardModel.Width - 1) { CheckTile(x + 1, y, gridCopy, true); }
        }
        else
        {
            if (y > 0) { CheckTile(x, y - 1, gridCopy, false); }
            if (y < boardModel.Height - 1) { CheckTile(x, y + 1, gridCopy, false); }
        }
    }


    private void AddToMatchingTilesInRow(int x, int y)
    {
        matchingTilesInRow.Add(gridCopy1[x, y]);
    }

    private void AddToMatchingTilesInCol(int x, int y)
    {
        matchingTilesInCol.Add(gridCopy2[x, y]);
    }

    private void RemoveMatchingTiles(List<GameObject> matchingTilesList)
    {
        foreach (var tile in matchingTilesList)
        {
            int x = GetCorrectPosition(tile.transform.position).x;
            int y = GetCorrectPosition(tile.transform.position).y;
            GameObject gridCell = grid[x, y];
            if (gridCell)
            {
                gridCell.GetComponent<Tile>().Destroy();
                gridCell = null;
            }
        }
        clearedTiles = true;
    }

    private void FillEmptyCells(List<GameObject> emptyCells)
    {
        foreach (var tile in emptyCells)
        {
            int x = GetCorrectPosition(tile.transform.position).x;
            int y = GetCorrectPosition(tile.transform.position).y;
            grid[x, y] = InstantiateTile(x, y);
        }
    }

    private GameObject InstantiateTile(int x, int y)
    {
        GameObject newTile = Instantiate(tilePrefab, new Vector3(x + startingBoardPos.x, y + startingBoardPos.y, 0f),
                                                 tilePrefab.transform.rotation, boardHandler.transform);

        newTile.GetComponent<SpriteRenderer>().color = GetRandomMismatchColor(x, y);
        newTile.name = x.ToString() + ' ' + y.ToString();
        return newTile;
    }

    private void CopyGrid(GameObject[,] sourceGrid)
    {
        gridCopy1 = new GameObject[boardModel.Width, boardModel.Height];
        gridCopy2 = new GameObject[boardModel.Width, boardModel.Height];
        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                gridCopy1[x, y] = sourceGrid[x, y];
                gridCopy2[x, y] = sourceGrid[x, y];
            }
        }
    }

    private Vector2Int GetCorrectPosition(Vector3 pos)
    {
        return new Vector2Int((int)pos.x - startingBoardPos.x, (int)pos.y - startingBoardPos.y);
    }

    private void OnDestroy()
    {
        boardView.OnButtonClicked -= CreateBoard;
        boardView.OnSwapTiles -= SwapTiles;
    }
}