using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour, IBoardView
{
    public event EventHandler OnSpaceClicked;
    public event EventHandler<int> OnEclicked;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("View action - Space");
            OnSpaceClicked(this, EventArgs.Empty);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("View action - E");
            OnEclicked(this, 2);
        }
    }
}
