using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, IBoardView
{
    public event EventHandler<ButtonClickedEventArgs> OnButtonClicked;
    public Vector2Int StartingBoardPosition { get => this.startingBoardPosition; set => this.startingBoardPosition = value; }
    public GameObject TilePrefab { get => this.tilePrefab; set => this.tilePrefab = value; }

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] protected Vector2Int startingBoardPosition;
    [SerializeField] protected GameObject tilePrefab;


    private void Awake()
    {
        startButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(false);
    }

    public void StartButton()
    {
        OnButtonClicked(this, new ButtonClickedEventArgs(this.startingBoardPosition, this.tilePrefab));
    }

    public void RestartButton()
    {
        OnButtonClicked(this, new ButtonClickedEventArgs(this.startingBoardPosition, this.tilePrefab));
    }
}
