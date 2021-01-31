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
        int firstX = (int)e.firstTilePosition.x - startingBoardPos.x;
        int firstY = (int)e.firstTilePosition.y - startingBoardPos.y;
        int secondX = (int)e.secondTilePosition.x - startingBoardPos.x;
        int secondY = (int)e.secondTilePosition.y - startingBoardPos.y;

        var tempTile = grid[firstX, firstY];
        var tempTileName = tempTile.name;

        grid[firstX, firstY].name = grid[secondX, secondY].name;
        grid[secondX, secondY].name = tempTileName;
        grid[firstX, firstY] = grid[secondX, secondY];
        grid[secondX, secondY] = tempTile;

        FindMatchingTiles(grid);
    }

    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
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
        colorList.AddRange(boardModel.Colors);

        if (currentX > 1) { colorList.Remove(grid[currentX - 2, currentY].GetComponent<SpriteRenderer>().color); }
        if (currentY > 1) { colorList.Remove(grid[currentX, currentY - 2].GetComponent<SpriteRenderer>().color); }

        int randomColor = UnityEngine.Random.Range(0, colorList.Count);
        return colorList[randomColor];
    }

    private void RemoveRepeatingColor(List<Color32> _colorList, int x, int y, Color32 _colorToRemove)
    {
        Color32 colorToRemove = grid[x, y].GetComponent<SpriteRenderer>().color;

        foreach (var color in _colorList)
        {
            if (color.Equals(colorToRemove))
            {
                _colorList.Remove(colorToRemove);
            }
        }
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
                    RemoveMatchingTilesInRow();
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
                    RemoveMatchingTilesInCol();
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

    private void RemoveMatchingTilesInRow()
    {
        var first = matchingTilesInRow[0].transform.position.y;
        foreach (var tile in matchingTilesInRow)
        {
            if (tile.transform.position.y == first)
            {
                int x = (int)tile.transform.position.x - startingBoardPos.x;
                int y = (int)tile.transform.position.y - startingBoardPos.y;
                GameObject gridCell = grid[x, y];
                if (gridCell)
                {
                    gridCell.GetComponent<Tile>().Destroy();
                    gridCell = null;
                }
            }
        }
        clearedTiles = true;
    }

    private void RemoveMatchingTilesInCol()
    {
        var first = matchingTilesInCol[0].transform.position.x;
        foreach (var tile in matchingTilesInCol)
        {
            if (tile.transform.position.x == first)
            {
                int x = (int)tile.transform.position.x - startingBoardPos.x;
                int y = (int)tile.transform.position.y - startingBoardPos.y;
                GameObject gridCell = grid[x, y];
                if (gridCell)
                {
                    gridCell.GetComponent<Tile>().Destroy();
                    gridCell = null;
                }
            }
        }
        clearedTiles = true;
    }

    private void FillEmptyCells(List<GameObject> emptyCells)
    {
        foreach (var tile in emptyCells)
        {
            int x = (int)tile.transform.position.x - startingBoardPos.x;
            int y = (int)tile.transform.position.y - startingBoardPos.y;
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

    private void OnDestroy()
    {
        boardView.OnButtonClicked -= CreateBoard;
        boardView.OnSwapTiles -= SwapTiles;
    }
}