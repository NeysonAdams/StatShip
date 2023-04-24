using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public interface Iview
{

    public void Show();
    public void Hide();
}

public interface IMainMenuView : Iview
{
    public event Action newGameButtonEvent;
    public event Action quickMatchButtonEvent;
}


public class MainMenuView : MonoBehaviour, IMainMenuView
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button quickMatchGameButton;

    public event Action newGameButtonEvent;
    public event Action quickMatchButtonEvent;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    void Start()
    {
        newGameButton.onClick.AddListener(() => newGameButtonEvent?.Invoke());
        quickMatchGameButton.onClick.AddListener(() => quickMatchButtonEvent?.Invoke());
    }

}
