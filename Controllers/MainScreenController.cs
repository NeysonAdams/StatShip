using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenController 
{
    private IMainMenuView screenView;
    private IMapView mapView;
    private IUiView uiview;

    private Level model;
    private LevelGenerator levelGenerator;
    private LevelBuilder builder;
    private GamePlayController gameplay;

    public MainScreenController(IMainMenuView _screenView, IMapView _mapView, IUiView _uiView, LevelGenerator _levelGenerator, Level _model, LevelBuilder _builder, GamePlayController _gameplay)
    {
        screenView = _screenView;
        mapView = _mapView;

        levelGenerator = _levelGenerator;
        model = _model;
        builder = _builder;
        gameplay = _gameplay;
        uiview = _uiView;

        Init();
    }

    private void Init()
    {
        screenView.newGameButtonEvent += () =>
        {
            screenView.Hide();
            mapView.Show();
        };
        screenView.quickMatchButtonEvent += () =>
        {
            screenView.Hide();
            mapView.Hide();
            model.level = 11;
            levelGenerator.Generate();
            gameplay.ActivateJoystick();
            builder.PushLevel();
        };

        mapView.backButtonEvent += () =>
        {
            mapView.Hide();
            screenView.Show();
        };

        mapView.levelsButtonsEvent += (level) =>
        {
            mapView.Hide();
            model.level = level+1;
            levelGenerator.Generate();
            builder.PushLevel();
            gameplay.ActivateJoystick();
        };

        uiview.startGameEvent += () =>
        {
            screenView.Hide();
            mapView.Hide();
            levelGenerator.Generate();
            gameplay.ActivateJoystick();
            builder.PushLevel();
        };
    }
}
