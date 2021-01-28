using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : Board
{
    [SerializeField] private BoardModel boardModel;
    [SerializeField] private BoardView boardView;

    private GameObject boardHandler;
    private GameObject[,] grid;

    public void Start()
    {
        SetSeed(boardModel.Seed);
        boardView.OnSpaceClicked += CreateBoard;
    }
    
    private void SetSeed(int seed)
    {
        UnityEngine.Random.InitState(seed);
    }

    private void CreateBoard(object sender, EventArgs e)
    {
        if(boardHandler == null)
        {
            boardHandler = new GameObject("BoardHandler");
            boardHandler.transform.parent = this.gameObject.transform;
        }
        else
        {
            Destroy(boardHandler);
        }
        
        grid = new GameObject[boardModel.Width, boardModel.Height];
        GameObject tile = boardModel.TilePrefab;

        for (int x = 0; x < boardModel.Width; x++)
        {
            for (int y = 0; y < boardModel.Height; y++)
            {
                GameObject newTile = Instantiate(tile, GetNextPosition(new Vector3(x, y, 0f)),
                                                 tile.transform.rotation, boardHandler.transform);

                
                newTile.GetComponent<SpriteRenderer>().color = GetRandomMismatchColor(x, y);
                grid[x, y] = newTile;
            }
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
        boardView.OnSpaceClicked -= CreateBoard;
    }
}
