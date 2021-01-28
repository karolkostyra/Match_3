using UnityEngine;

[System.Serializable]
public class BoardModel : IBoardModel
{
    public int Width { get => this.width; set => this.width = value; }
    public int Height { get => this.height; set => this.height = value; }
    public int Seed { get => this.seed; set => this.seed = value; }
    public int ColorCount { get => this.colorCount; set => this.colorCount = value; }
    public Color32[] Colors { get => this.colors; set => this.colors = value; }
    public int CellSize { get => this.cellSize; set => this.cellSize = value; }
    public GameObject TilePrefab { get => this.tilePrefab; set => this.tilePrefab = value; }


    [SerializeField] protected int width;
    [SerializeField] protected int height;
    [SerializeField] protected int seed;
    [SerializeField] protected int colorCount;
    [SerializeField] protected Color32[] colors;
    [SerializeField] protected int cellSize;
    [SerializeField] protected GameObject tilePrefab;
}
