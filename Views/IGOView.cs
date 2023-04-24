using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public interface IGOView
{
    public void SetModel(Model model);
    public void Shoot();
    public void Move(object[] args);
    public Transform GetTransform();
    public void SetSounds(ISoundHolder holder);
}

public interface Imatrix
{
    public Vector3 GetMatrixPosition(int x, int y);
    public Transform GetMatrixElement(int x, int y);
    public Transform GetPrarent();
    public Transform GetTransform();
    public void StartMatrixAnimation();
}

public interface ILevelView: IGOView
{
    public event Action<GAMEPHASE> SwitchPhase;
    public event Action<IEnemy, Vector3[], int> SetAnimation;
    public event Action<int> scoreUp;
    public event Action<Vector3> bonusEvent;
    public void Push(GAMEPHASE key, Action phase);
    public bool IsNoEnemies();
    public IGOView GEtRandomEnemy();


}

public interface IEnemy : IGOView
{
    public event Action<IEnemy> movemetFinish;
    public event Action<IEnemy> destroy;
    public event Action<int> scoreUp;
    public Model GetModel();

}

public interface IUiView
{
    public event Action<Vector3> readJoystickBehaviour;
    public event Action<JoystickView> onMouseDowm;
    public event Action<JoystickView> onMouseUp;
    public event Action<JoystickView> onMouseDrag;
    public event Action<bool> onPause;
    public event Action<int> addScore;
    public event Action<float> sfxSliderEvent;
    public event Action<float> musikSliderEvent;
    public event Action<bool> sfxToggleEvent;
    public event Action<bool> musikToggleEvent;
    public event Action startGameEvent;

    public Vector2 GetMouseCoordinate();
    public Transform GetParrent();
    public void SetJoystick(JoystickView value);
    public void ShowMessage(string message, float duration);
    public void ShowGamePlayUI();
    public void ShowMessageSequansee(string[] messages);
    public void AddScore(int score);
    public void AddCoin(int coin);
    public void SetLives(int lives_num);
    public void ShowGameOver();
    public void ShowWin(string score, string coins);
}

public interface IPlayer : IGOView
{
    public event Action<Transform> destroy;
    public event Action addCoin;
    public event Action<int> setLives;
    public void PutonStartPoint();

    public void AddScore(int score);
    public Model GetModel();
    public void SetPause(bool _pause);
}
