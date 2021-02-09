using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : Board, IBoardController
{
    [SerializeField] private BoardModel boardModel;
    [SerializeField] private BoardView boardView;
    [SerializeField] private GameObject spriteMaskPrefab;

    private GameObject boardHandler;
    private GameObject currentTile;
    private GameObject[,] grid, gridCopy1, gridCopy2;
    private List<GameObject> matchingTilesInRow, matchingTilesInCol;
    private bool clearedTiles;
    private GameObject tilePrefab;
    private Vector2Int startingBoardPos;
    private bool fullGrid;
    private int counter = 0;
    private bool isMoving = false;

    public void Awake()
    {
        SetSeed(boardModel.Seed);
        boardView.OnButtonClicked += CreateBoard;
        boardView.OnSwapTiles += SwapTiles;
        matchingTilesInRow = new List<GameObject>();
        matchingTilesInCol = new List<GameObject>();
    }

    private void Update()
    {
        if (fullGrid && clearedTiles && !isMoving)
        {
            boardView.isMoving = false;
            FindMatchingTiles(grid);
        }
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

        if (fullGrid)
        {
            FindMatchingTiles(grid);
        }
    }

    private void SetSeed(int seed)
    {
        Random.InitState(seed);
    }

    private void CreateBoard(object sender, CreateBoardEventArgs e)
    {
        CheckBoardHandler();
        grid = new GameObject[boardModel.Width, boardModel.Height+1];
        startingBoardPos = e.startingBoardPos;
        tilePrefab = e.tilePrefab;

        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height+1; y++)
            {
                grid[x, y] = InstantiateTile(x, y);
            }
        }
        GameObject spriteMask = Instantiate(spriteMaskPrefab, grid[boardModel.Width/2,boardModel.Height].transform.position,
                                            spriteMaskPrefab.transform.rotation, boardHandler.transform);

        spriteMask.transform.localScale = new Vector3(boardModel.Width + boardModel.Width * startingBoardPos.x,
                                                      tilePrefab.transform.localScale.y, 0);
        fullGrid = true;
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
        if (currentX > 1) { colorList.Remove(GetTileColor(grid[currentX - 2, currentY])); maxRange--; }
        if (currentY > 1) { colorList.Remove(GetTileColor(grid[currentX, currentY - 2])); maxRange--; }
        
        int randomColor = Random.Range(0, maxRange);
        return colorList[randomColor];
    }

    private void FindMatchingTiles(GameObject[,] grid)
    {
        CopyGrid(grid);
        clearedTiles = false;
        currentTile = null;
        matchingTilesInRow.Clear();
        matchingTilesInCol.Clear();

        for (int y = 0; y < boardModel.Height; y++)
        {
            for (int x = 0; x < boardModel.Width; x++)
            {
                CheckTile(x, y, gridCopy1, matchingTilesInRow);
                CheckTile(x, y, gridCopy2, matchingTilesInCol, false);

                RemoveMatchingTiles(matchingTilesInRow);
                RemoveMatchingTiles(matchingTilesInCol);
                
                currentTile = null;
                matchingTilesInRow.Clear();
                matchingTilesInCol.Clear();
            }
        }
        if (fullGrid && clearedTiles)
        {
            FillEmptyCells();
        }
    }

    private void CheckTile(int x, int y, GameObject[,] gridCopy, List<GameObject> matchingTiles, bool inRow = true)
    {
        if (gridCopy[x, y] == null)
        {
            return;
        }
        if (currentTile == null)
        {
            currentTile = gridCopy[x, y];
            matchingTiles.Add(gridCopy[x,y]);
            gridCopy[x, y] = null;
        }
        else if (GetTileColor(currentTile) == GetTileColor(gridCopy[x, y]))
        {
            matchingTiles.Add(gridCopy[x, y]);
            gridCopy[x, y] = null;
        }
        else
        {
            return;
        }

        if (inRow)
        {
            if (x > 0) { CheckTile(x - 1, y, gridCopy, matchingTiles, true); }
            if (x < boardModel.Width - 1) { CheckTile(x + 1, y, gridCopy, matchingTiles, true); }
        }
        else
        {
            if (y > 0) { CheckTile(x, y - 1, gridCopy, matchingTiles, false); }
            if (y < boardModel.Height - 1) { CheckTile(x, y + 1, gridCopy, matchingTiles, false); }
        }
    }

    private void RemoveMatchingTiles(List<GameObject> matchingTilesList)
    {
        if (matchingTilesList.Count < 3)
        {
            return;
        }
        foreach (var tile in matchingTilesList)
        {
            int x = GetCorrectPosition(tile.transform.position).x;
            int y = GetCorrectPosition(tile.transform.position).y;
            GameObject gridCell = grid[x, y];
            if (gridCell)
            {
                gridCell.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
            }
        }
        clearedTiles = true;
        fullGrid = false;
        FillEmptyCells();
    }

    private void FillEmptyCells()
    {
        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                if ((grid[x, y] == null || GetTileColor(grid[x, y])== new Color32(0, 0, 0, 0)) && !isMoving)
                {
                    counter++;
                    int x2 = (int)GetNearestTilePos(x, y).x;
                    int y2 = (int)GetNearestTilePos(x, y).y;

                    var temp = grid[x2, y2];
                    var tempName = temp.name;
                    temp.name = grid[x, y].name;
                    grid[x, y].name = tempName;
                    grid[x2, y2] = null;

                    StartCoroutine(MoveTileDown(x, y, x2, y2, temp));
                    break;
                }
                counter = 0;
            }
            if (counter > 0)
            {
                break;
            }
        }
        if(counter == 0)
        {
            fullGrid = true;
            clearedTiles = true;
        }
        else
        {
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);
        FillEmptyCells();
    }

    public IEnumerator MoveTileDown(int x1, int y1, int x2, int y2, GameObject movingTile)
    {
        isMoving = boardView.isMoving = true;
        float i = 0;
        var tile1 = grid[x1, y1].transform.position;
        var tile2 = movingTile.transform.position;

        while (i < 1)
        {
            i += Time.deltaTime * 15f;
            grid[x1, y1].transform.position = Vector3.Lerp(grid[x1, y1].transform.position, tile2, i);
            movingTile.transform.position = Vector3.Lerp(movingTile.transform.position, tile1, i);
            yield return 0;
        }
        var temp = grid[x1, y1];
        grid[x1, y1] = movingTile;
        grid[x2, y2] = temp;
        UpdateAdditionalRow();
        isMoving = false;
    }

    private void UpdateAdditionalRow()
    {
        int y = boardModel.Height;
        for (int x = 0; x < boardModel.Width; x++)
        {
            if(GetTileColor(grid[x, y]) == new Color32(0, 0, 0, 0))
            {
                grid[x, y].GetComponent<SpriteRenderer>().color = GetRandomMismatchColor(x, y);
            }
        }
    }

    private Vector3 GetNearestTilePos(int x, int y)
    {
        int nearestTileY = 0;
        for (int i = y; i < boardModel.Height+1; i++)
        {
            if(grid[x,i] != null && GetTileColor(grid[x,i]) != new Color32(0,0,0,0))
            {
                nearestTileY = i;
                break;
            }
        }
        return new Vector3(x, nearestTileY, 0);

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

    private Color GetTileColor(GameObject tile)
    {
        return tile.GetComponent<SpriteRenderer>().color;
    }

    private void OnDestroy()
    {
        boardView.OnButtonClicked -= CreateBoard;
        boardView.OnSwapTiles -= SwapTiles;
    }
}