using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class UIView : MonoBehaviour, IUiView
{
    [SerializeField] Camera camera;
    [SerializeField] JoystickView joystickView;

    [Header("GamePlayUI")]
    [SerializeField] private CanvasGroup ScoreBox;
    [SerializeField] private TextMeshProUGUI ScoreLabel;
    [SerializeField] private CanvasGroup CoinBox;
    [SerializeField] private TextMeshProUGUI CoinLabel;
    [SerializeField] private CanvasGroup MessageBox;
    [SerializeField] private TextMeshProUGUI MessageLabel;
    [SerializeField] private CanvasGroup LiveBox;
    [SerializeField] private Transform LiveIcon;
    [SerializeField] private MenuView Menu;

    [Header("Button")]
    [SerializeField] private Button PauseButton;

    [Header("Sounds")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musikSlider;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle musikToggle;

    public event Action<Vector3> readJoystickBehaviour;
    public event Action<JoystickView> onMouseDowm;
    public event Action<JoystickView> onMouseUp;
    public event Action<JoystickView> onMouseDrag;
    public event Action<bool> onPause;
    public event Action<int> addScore;
    public event Action nextButtonClickEvent;
    public event Action startGameEvent;

    public event Action<float> sfxSliderEvent;
    public event Action<float> musikSliderEvent;
    public event Action<bool> sfxToggleEvent;
    public event Action<bool> musikToggleEvent;

    private JoystickView m_jView;
    private List<Transform> lives = new List<Transform>();
    private bool pause = false;

    private void Start()
    {
        ScoreBox.alpha = 0;
        CoinBox.alpha = 0;
        LiveBox.alpha = 0;
        PauseButton.onClick.AddListener(()=>{
            Menu.gameObject.SetActive(true);
            Menu.ShowPause();
            onPause?.Invoke(true);
        });

        Menu.setRestartButton(() =>
        {
            SceneManager.LoadScene("Game");
            startGameEvent?.Invoke();
        });

        Menu.setExitButton(() =>
        {
            SceneManager.LoadScene("Game");
        });

        Menu.setNextButton(() =>
        {
            nextButtonClickEvent?.Invoke();
            SceneManager.LoadScene("Game");
            startGameEvent?.Invoke();
        });

        Menu.setContinueButton(() =>
        {
            onPause?.Invoke(false);
        });

        sfxSlider.onValueChanged.AddListener((value) =>
        {
            sfxSliderEvent?.Invoke(value);
        });

        musikSlider.onValueChanged.AddListener((value) =>
        {
            musikSliderEvent?.Invoke(value);
        });

        sfxToggle.onValueChanged.AddListener((value) =>
        {
            sfxToggleEvent?.Invoke(!value);
        });

        musikToggle.onValueChanged.AddListener((value) =>
        {
            musikToggleEvent?.Invoke(!value);
        });
    }

    public void ShowGameOver()
    {
        Menu.gameObject.SetActive(true);
        Menu.ShowGameOver();
        onPause?.Invoke(true);
    }

    public void ShowWin(string score, string coins)
    {
        Menu.gameObject.SetActive(true);
        Menu.ShowWin(score, coins);
        onPause?.Invoke(true);
    }

    public void AddScore(int score)
    {
        int old_score = int.Parse(ScoreLabel.text);
        TweenController.Instance.ChangeText((old_score + score).ToString(), ScoreLabel);
        addScore?.Invoke(score);
    }

    public void AddCoin(int coin)
    {
        int old_coin = int.Parse(CoinLabel.text);
        TweenController.Instance.ChangeText((old_coin + coin).ToString(), CoinLabel);
    }

    public void SetLives(int lives_num)
    {
        for (int i =0; i<lives.Count; i++)
            Destroy(lives[i].gameObject);
        lives.Clear();
        for (int i = 0; i < lives_num; i++)
        {
            Transform live_con = (Transform)Instantiate(LiveIcon, LiveBox.transform);
            live_con.gameObject.SetActive(true);
            lives.Add(live_con);
        }
    }

    public void ShowGamePlayUI()
    {
        TweenController.Instance.FadeIn(ScoreBox);
        TweenController.Instance.FadeIn(CoinBox);
        TweenController.Instance.FadeIn(LiveBox);
    }
        

    public void ShowMessage(string message, float duration)
    {
        MessageLabel.text = message;
        TweenController.Instance.ShowMessage(MessageBox, duration);
    }

    public void ShowMessageSequansee(string[] messages)
    {
        TweenController.Instance.ShowMessageSequance(messages, MessageLabel, MessageBox);
    }

    public Vector2 GetMouseCoordinate()
    {
        Vector2 mouse_pos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, camera, out mouse_pos);
        return mouse_pos;
    }


    public Transform GetParrent()
    {
        return transform;
    }
    public void SetJoystick(JoystickView value)
    {
        m_jView = value;
    }

    private void OnMouseDown()
    {
        onMouseDowm?.Invoke(joystickView);
    }

    private void OnMouseUp()
    {
        onMouseUp?.Invoke(m_jView);
        this.readJoystickBehaviour?.Invoke(Vector3.zero);
    }

    private void OnMouseDrag()
    {
        onMouseDrag?.Invoke(m_jView);
        this.readJoystickBehaviour?.Invoke(m_jView.Movement);

    }
}
