using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayController
{
    private IUiView uiview;
    private IPlayer player;
    private ISoundHolder sounds;



    public GamePlayController(IUiView _uiview, IPlayer _player, ISoundHolder holder, Model p_model)
    {
        this.uiview = _uiview;
        this.player = _player;
        uiview.SetLives((p_model as PlayerModel).lives);
        this.player.SetModel(p_model);
        player.SetSounds(holder);
        this.sounds = holder;
        Init();
    }

    private void Init()
    {
        player.addCoin += () => uiview.AddCoin(1);
        player.setLives += (lives) => uiview.SetLives(lives);
        uiview.addScore += player.AddScore;

        uiview.onPause += (pause) =>
        {
            if (pause)
                DeactivateJoystick();
            else
                ActivateJoystick();
            player.SetPause(pause);
            TweenController.Instance.SetPause(pause);
        };

        uiview.sfxSliderEvent += (value) => sounds.ChangeSFXVolume(value);
        uiview.sfxToggleEvent += (value) => sounds.SetMuteSFX(value);
        uiview.musikSliderEvent += (value) => sounds.ChangeMusikVolume(value);
        uiview.musikToggleEvent += (value) => sounds.SetMuteSFX(value);
    }

    private void DeactivateJoystick()
    {
        uiview.onMouseDowm -= MouseDown;
        uiview.onMouseUp -= MouseUp;
        uiview.onMouseDrag -= MouseDrag;
        uiview.readJoystickBehaviour -= readJoystick;
    }

    public void ActivateJoystick()
    {
        uiview.onMouseDowm += MouseDown;
        uiview.onMouseUp += MouseUp;
        uiview.onMouseDrag += MouseDrag;
        uiview.readJoystickBehaviour += readJoystick;
    }

    #region Joystick Controller
    private void readJoystick(Vector3 movement)
    {
        this.player.Move(new object[] { ConvertToPlayerFormat(movement) });
    }

    private Vector2 ConvertToPlayerFormat(Vector3 movement)
    {
        return new Vector2(movement.x / 80, movement.y / 80);
    }

    

    private void MouseDown(JoystickView joystick)
    {
        JoystickView m_jView;
        m_jView = (JoystickView)Transform.Instantiate(joystick, uiview.GetParrent());
        m_jView.transform.localPosition = uiview.GetMouseCoordinate();
        m_jView.gameObject.name = "Joystick";
        uiview.SetJoystick(m_jView);
    }

    private void MouseUp(JoystickView joystick)
    {
        if (joystick != null)
            Transform.Destroy(joystick.gameObject);
    }

    private void MouseDrag(JoystickView joystick)
    {
        joystick.Movement = uiview.GetMouseCoordinate();
    }
    #endregion
}


