using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, IBoardView
{
    public event EventHandler OnSpaceClicked;
    public event EventHandler OnButtonClicked;


    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

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
        OnButtonClicked(this, EventArgs.Empty);
    }

    public void RestartButton()
    {
        OnButtonClicked(this, EventArgs.Empty);
    }
}
