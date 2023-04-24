using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MenuView : MonoBehaviour
{
    [SerializeField] private Transform PauseMenu;
    [SerializeField] private Transform GameOverMenu;
    [SerializeField] private Transform WinMenu;

    [SerializeField] private TextMeshProUGUI ScoreLabel;
    [SerializeField] private TextMeshProUGUI CoinsLabel;

    [SerializeField] private Button[] restart_button;
    [SerializeField] private Button[] exit_button;
    [SerializeField] private Button next_button;
    [SerializeField] private Button continue_button;

    public void setRestartButton(Action action)
    {
        for (int i = 0; i < restart_button.Length; i++)
            restart_button[i].onClick.AddListener(() => action?.Invoke());
    }

    public void setExitButton(Action action)
    {
        for (int i = 0; i < exit_button.Length; i++)
            exit_button[i].onClick.AddListener(() => action?.Invoke());
    }

    public void setNextButton(Action action)
    {
        next_button.onClick.AddListener(() => action.Invoke());
    }

    public void setContinueButton(Action action)
    {
        continue_button.onClick.AddListener(() => { 
            action.Invoke();
            Debug.Log("Continue Clicked");
            gameObject.SetActive(false);
            PauseMenu.gameObject.SetActive(false);
            GameOverMenu.gameObject.SetActive(false);
            WinMenu.gameObject.SetActive(false);
        });
    }

    public void ShowPause()
    {
        PauseMenu.gameObject.SetActive(true);
    }
    public void ShowGameOver()
    {
        GameOverMenu.gameObject.SetActive(true);
    }
    public void ShowWin(string score, string coins)
    {
        WinMenu.gameObject.SetActive(true);
        ScoreLabel.text = "Score : " + score;
        CoinsLabel.text = "Coin : " + coins;

    }
}
