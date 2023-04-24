using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CompositionRoot : MonoBehaviour
{
    [SerializeField] PlayerModel playerModel;
    [SerializeField] Level level;
    [SerializeField] RespownePoints respownemodel;
    [SerializeField] BonusModel bonusModel;
    //[SerializeField] EnemyView enemy;

    private UIView uiview;
    private PlayerViev player;
    private MatrixView matrix;
    private LevelView levelView;
    private MapView mapView;
    private MainMenuView screenView;

    private GamePlayController gamePlayController;
    private TweenController tweenController;
    private LevelBuilder levelBuilder;
    private BonusGenericHelper bonusGenericHelper;
    private LevelGenerator levelGenerator;
    private SoundHolder holder;
    private MainScreenController mainScreen;


    private void Awake()
    {
        uiview = GameObject.Find("UICanvas").GetComponent<UIView>();
        screenView = GameObject.Find("MainScreen").GetComponent<MainMenuView>();
        mapView = GameObject.Find("MapUI").GetComponent<MapView>();
        player = GameObject.Find("Player").GetComponent<PlayerViev>();
        matrix = GameObject.Find("Matrix").GetComponent<MatrixView>();
        levelView = GameObject.Find("GamePlay").GetComponent<LevelView>();
        holder = GameObject.Find("Sound").GetComponent<SoundHolder>();

        tweenController = new TweenController();
        bonusGenericHelper = new BonusGenericHelper(bonusModel);
        levelGenerator = new LevelGenerator(level);
    }

    void Start()
    {
        
        

        gamePlayController = new GamePlayController(uiview, player, holder, playerModel);
        
        levelBuilder = new LevelBuilder(level, levelView, player, matrix, uiview, holder, bonusGenericHelper, respownemodel);

        mainScreen = new MainScreenController(screenView, mapView, uiview, levelGenerator, level, levelBuilder, gamePlayController);


    }

}
