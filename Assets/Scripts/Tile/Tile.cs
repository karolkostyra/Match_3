using System;
using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    public BoardController BoardController { get => this.boardController; set => boardController = value; }

    [SerializeField] protected BoardController boardController;

    [SerializeField] private static Tile tileSelected;

    private void OnMouseDown()
    {
        BoardController.Instance.GetTile(this.gameObject);

        /*
        if(tileSelected != null)
        {
            if(tileSelected == this)
            {
                return;
            }
            if(Vector3.Distance(tileSelected.transform.position, this.transform.position) == 1)
            {
                BoardController.Instance.SwapTiles(tileSelected.transform.position, this.gameObject.transform.position);
                tileSelected = null;
            }
            else
            {
                tileSelected = this;
            }
        }
        else
        {
            tileSelected = this;
        }
        */


        /*
        if(tileSelected == null)
        {
            Select();
            Debug.Log("null - " + tileSelected.name);
        }
        else
        {
            if(tileSelected != this)
            {
                Debug.Log("swap");
                BoardController.Instance.SwapTiles(tileSelected.transform.position, this.gameObject.transform.position);
            }
            Deselect();
        }
        */
    }

    private void Select()
    {
        tileSelected = this;
    }

    public void Deselect()
    {
        tileSelected.Deselect();
        tileSelected = null;
    }
}
