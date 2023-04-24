using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelView : MonoBehaviour, ILevelView
{
    [SerializeField]
    private List<EnemyView> prefabs = new List<EnemyView>();
    [SerializeField]
    private List<EnemyView> asteroids = new List<EnemyView>();

    private List<IEnemy> enemies = new List<IEnemy>();
    private Tween currentTween;
    private ISoundHolder sounds;

    public event Action<GAMEPHASE> SwitchPhase;
    public event Action<IEnemy, Vector3[], int> SetAnimation;
    public event Action<int> scoreUp;
    public event Action<Vector3> bonusEvent;



    public void AddEnemyLine(Model level, Vector3 resp_point, Vector3[] path)
    {
        Level m_level = level as Level;

        var enemiesModels = m_level.waves[m_level.current_wave-1].enemies;
        
        for (int i = 0; i < enemiesModels.Count; i++)
        {
            int v = i;
            TweenController.Instance.DoSimpleStep(
                null, 0.3f * i, () =>
                {
                    var model = enemiesModels[v];
                    EnemyView elemet = null;
                    if (m_level.waves[m_level.current_wave - 1].type != WaveType.ARMAGEDON)
                        elemet = (EnemyView)Instantiate(prefabs[(int)model.enemy_type], transform);
                    else
                        elemet = (EnemyView)Instantiate(asteroids[(int)UnityEngine.Random.Range(0, 2)], transform);
                    model.id = (m_level.waves[m_level.current_wave].currrent_line - 1) * 5 + v;
                    elemet.transform.position = resp_point;
                    elemet.SetModel(model);
                    elemet.destroy += (t) => enemies.Remove(t);
                    elemet.scoreUp += scoreUp;
                    elemet.bonusEvent += bonusEvent;
                    elemet.SetSounds(sounds);
                    enemies.Add(elemet);
                    SetAnimation?.Invoke(elemet, path, enemies.Count);
                    if (v == enemiesModels.Count - 1)
                        m_level.waves[m_level.current_wave].currrent_line++;
                });
            
        }
        

    }

    public void Shoot()
    {
        for (int i = 0; i < 7; i++)
            TweenController.Instance.DoSimpleStep(null, UnityEngine.Random.Range(0, 10f),
                () =>
                {
                    if (enemies.Count > 0) enemies[UnityEngine.Random.Range(0, enemies.Count - 1)].Shoot();
                });
    }

    public IGOView GEtRandomEnemy()
    {
        return enemies[UnityEngine.Random.Range(0, enemies.Count - 1)];
    }

    public void Push(GAMEPHASE key, Action phase)
    {
        Action prev_step = null, post_step = null;
        Action <Sequence> update = null ;
        int[] intervals = new int[] { 0, 5, 5, 10, 3, 5, 5 };
        float interval = intervals[(int)key];
        int lll = 0;
        if (key == GAMEPHASE.PLAY_MODE)
        {
            prev_step = null;
            update += (tween) =>
            {
                if (enemies.Count == 0)
                {
                    lll++;
                    if (lll == 1)
                    {
                        tween.Kill();
                        SwitchPhase?.Invoke(key);
                        
                    }
                }
            };
            post_step += () =>
            {
                if (enemies.Count > 0)
                    phase?.Invoke();
                SwitchPhase?.Invoke(key);

            };
        }
        else
        {
            prev_step += phase;
            post_step += () => SwitchPhase?.Invoke(key);
        }
        Debug.Log(key + "  " + interval);
        TweenController.Instance.DoSimpleStep(prev_step, interval, post_step, update);
        
    }


    public bool IsNoEnemies()
    {
        return enemies.Count == 0;
    }

    public void SetModel(Model model)
    {
        throw new NotImplementedException();
    }

    public void Move(object[] args)
    {
        AddEnemyLine(args[0] as Model, (Vector3)args[1], (Vector3[])args[2]);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void SetSounds(ISoundHolder holder)
    {
        sounds = holder;
    }
}
