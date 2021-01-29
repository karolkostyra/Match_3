using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, ITile
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        BoardController.Instance.GetSelectedTile(this.gameObject);
    }

    public void Select()
    {
        Color32 temp = spriteRenderer.color;
        spriteRenderer.color = new Color32(temp.r, temp.g, temp.b, 150);
    }

    public void Deselect()
    {
        Color32 temp = spriteRenderer.color;
        spriteRenderer.color = new Color32(temp.r, temp.g, temp.b, 255);
    }
}
