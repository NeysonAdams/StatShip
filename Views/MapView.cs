using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public interface IMapView : Iview
{
    public event Action<int> levelsButtonsEvent;
    public event Action backButtonEvent;
}

public class MapView : MonoBehaviour , IMapView
{
    [SerializeField] List<Button> levelButtons;
    [SerializeField] Button backButton;

    public event Action<int> levelsButtonsEvent;
    public event Action backButtonEvent;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i<levelButtons.Count; i++)
        {
            int c = 0;
            levelButtons[i].onClick.AddListener(() => levelsButtonsEvent?.Invoke(c));
        }
        backButton.onClick.AddListener(() => backButtonEvent?.Invoke());

    }

}
