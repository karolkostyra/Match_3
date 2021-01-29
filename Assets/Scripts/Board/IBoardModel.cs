using UnityEngine;

public interface IBoardModel
{
    int Width { get; set; }
    int Height { get; set; }
    int Seed { get; set; }
    int ColorCount { get; set; }
    Color32[] Colors { get; }
}
