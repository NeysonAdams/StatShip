using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public enum GAMEPHASE
{
    NONE,
    START_GAME,
    ADD_ENEMY_LINE,
    PLAY_MODE,
    NEXT_WAVE,
    WIN,
    LOSE,

}

public class LevelBuilder
{
    private ILevelView view;
    private IPlayer player;
    private Imatrix matrix;
    private IUiView uiview;
    public ISoundHolder sounds;

    private Level model;
    private Dictionary<GAMEPHASE, Action> phases = new Dictionary<GAMEPHASE, Action>();
    private RespownePoints respownemodel;
    private BonusGenericHelper bonusHelper;
    private LevelGenerator levelGenerator;

    public LevelBuilder(Model _model, ILevelView _view, IPlayer _player, Imatrix _matrix, IUiView _uiview, ISoundHolder holder, BonusGenericHelper _bonusHelper, RespownePoints _respownemodel)
    {
        this.model = _model as Level;
        view = _view;
        player = _player;
        matrix = _matrix;
        uiview = _uiview;
        bonusHelper = _bonusHelper;
        sounds = holder;
        respownemodel = _respownemodel;

        view.SetSounds(holder);

        Init();

    }

    public void Init()
    {

        view.scoreUp += (score) => uiview.AddScore(score);

        view.bonusEvent += (position) =>
        {
            Transform bonus = bonusHelper.DropBonus();
            if(bonus != null)
            {
                Transform b = (Transform)Transform.Instantiate(bonus, player.GetTransform().parent);
                b.position = position;
            }
        };

        view.SetAnimation += (enemy, path, len) =>{
            float pos = enemy.GetTransform().position.x;
            int x = model.waves[model.current_wave].currrent_line - 1;
            int y = pos>=0 ? enemy.GetModel().id - x * 5 : 4 - (enemy.GetModel().id - x * 5);
            WaveType type = model.waves[model.current_wave].type;
            enemy.Move(new object[]
            {
                (type == WaveType.MATRIX) ? AnimationType.ENEMY_PUSH_MATRIX : AnimationType.ENEMY_LINE,
                matrix,
                path,
                x, y
            });
           ;
        };

        view.SwitchPhase += (pahse) =>
        {
            GAMEPHASE m_g_pahse = GAMEPHASE.NONE;
            switch (pahse)
            {
                case GAMEPHASE.START_GAME:
                    m_g_pahse = GAMEPHASE.ADD_ENEMY_LINE;
                    view.Push(m_g_pahse, phases[m_g_pahse]);
                    break;
                case GAMEPHASE.ADD_ENEMY_LINE:
                    Debug.Log("CURRENT :: " + model.waves[model.current_wave].type);
                    Debug.Log(model.waves[model.current_wave].currrent_line + " : " + model.waves[model.current_wave].line_count + " :: "
                        + (model.waves[model.current_wave].currrent_line >= model.waves[model.current_wave].line_count));
                    if (model.waves[model.current_wave].currrent_line >= model.waves[model.current_wave].line_count)
                    {
                        model.waves[model.current_wave].currrent_line = 1;
                        m_g_pahse = GAMEPHASE.PLAY_MODE;
                    }
                    else
                        m_g_pahse = GAMEPHASE.ADD_ENEMY_LINE;
                    view.Push(m_g_pahse, phases[m_g_pahse]);
                    break;
                case GAMEPHASE.PLAY_MODE:
                    m_g_pahse = pahse;
                    if (model.waves_cout >= model.current_wave && view.IsNoEnemies())
                        m_g_pahse = GAMEPHASE.NEXT_WAVE;
                    view.Push(m_g_pahse, phases[m_g_pahse]);
                    break;
                case GAMEPHASE.NEXT_WAVE:
                    m_g_pahse = (model.waves_cout < model.current_wave) ? GAMEPHASE.WIN : GAMEPHASE.ADD_ENEMY_LINE;
                    view.Push(m_g_pahse, phases[m_g_pahse]);
                    break;
                case GAMEPHASE.WIN:

                    break;
                case GAMEPHASE.LOSE:

                    break;
            }
        };

        player.destroy += (t) =>
        {
            Debug.Log("Lose");
            view.Push(GAMEPHASE.LOSE, phases[GAMEPHASE.LOSE]);
        };

        AddPhase(GAMEPHASE.START_GAME, () =>
        {
            player.PutonStartPoint();
            matrix.StartMatrixAnimation();
            uiview.ShowGamePlayUI();
            uiview.ShowMessageSequansee(new string[] { "3", "2", "1", "GO" });
            sounds.PlayMusik(SoundsMSK.GAME, true);
        });

        AddPhase(GAMEPHASE.ADD_ENEMY_LINE, () =>
        {
            int rand = Mathf.RoundToInt(UnityEngine.Random.Range(0, respownemodel.respownePoints.Length-1));
            Vector3 r_point = respownemodel.respownePoints[rand];
            var pathes = (model.waves[model.current_wave].type == WaveType.MATRIX) ? respownemodel.in_paths : respownemodel.line_path;
            rand = Mathf.RoundToInt(UnityEngine.Random.Range(0, pathes.Count - 1));
            Vector3[] in_path = pathes[rand].path;
            view.Move(new object[] { model, r_point, in_path });


        });

        AddPhase(GAMEPHASE.PLAY_MODE, () =>
        {
            view.Shoot();
            IGOView enemy = view.GEtRandomEnemy();
            Vector3[] path = new Vector3[]
            {
                player.GetTransform().position,
                new Vector3(player.GetTransform().position.x,-6, player.GetTransform().position.z)
            };
            enemy.Move(new object[]
            {
                AnimationType.ENEMY_ATACK,
                player,
                path
            });
            
        });


        AddPhase(GAMEPHASE.NEXT_WAVE, () =>
        {
            Debug.Log("NEXT :: "+ model.waves[model.current_wave].type);
            model.current_wave += 1;
            if (model.waves_cout > model.current_wave) {
                uiview.ShowMessageSequansee(new string[] { "WAVE" + model.current_wave.ToString(), "GO" });
            }
        });

        AddPhase(GAMEPHASE.WIN, () =>
        {
            var model = player.GetModel() as PlayerModel;
            uiview.ShowWin(model.score.ToString(), model.coins.ToString());
            sounds.PlayMusik(SoundsMSK.WIN, false);
        });

        AddPhase(GAMEPHASE.LOSE, () =>
        {
            uiview.ShowGameOver();
            sounds.PlayMusik(SoundsMSK.LOSE, false);
        });

        
    }

    public void PushLevel()
    {
        view.Push(GAMEPHASE.START_GAME, phases[GAMEPHASE.START_GAME]);
    }



    #region Helpers


    public void AddPhase(GAMEPHASE key, Action phaseDelegate)
    {
        phases.Add(key, phaseDelegate);
    }
    
    #endregion
}


