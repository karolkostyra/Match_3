using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardModel
{
    int Width { get; set; }
    int Height { get; set; }
    int Seed { get; set; }
    int ColorCount { get; set; }
    List<Color32> Colors { get; set; }
}
