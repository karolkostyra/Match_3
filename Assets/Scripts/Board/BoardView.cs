using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, IBoardView
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] protected Vector2 startingBoardPosition;

    public event EventHandler OnSpaceClicked;
    public event EventHandler<Vector2> OnButtonClicked;
    public Vector2 StartingBoardPosition { get => this.startingBoardPosition; set => this.startingBoardPosition = value; }

    private void Awake()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("View action - Space");
            OnSpaceClicked(this, EventArgs.Empty);
        }
    }

    public void StartButton()
    {
        OnButtonClicked(this, this.startingBoardPosition);
    }

    public void RestartButton()
    {
        OnButtonClicked(this, this.startingBoardPosition);
    }
}
