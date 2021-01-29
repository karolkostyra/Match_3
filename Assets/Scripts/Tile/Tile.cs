using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    private void OnMouseDown()
    {
        BoardController.Instance.GetTile(this.gameObject);
    }
}
