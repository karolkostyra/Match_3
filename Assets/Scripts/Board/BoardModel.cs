using UnityEngine;

[System.Serializable]
public class BoardModel : IBoardModel
{
    public int Width { get => this.width; set => this.width = value; }
    public int Height { get => this.height; set => this.height = value; }
    public int Seed { get => this.seed; set => this.seed = value; }
    public int ColorCount { get => this.colorCount; set => this.colorCount = value; }
    public Color32[] Colors { get => this.colors; set => this.colors = value; }


    [SerializeField] protected int width;
    [SerializeField] protected int height;
    [SerializeField] protected int seed;
    [SerializeField] protected int colorCount;
    [SerializeField] protected Color32[] colors;
}
