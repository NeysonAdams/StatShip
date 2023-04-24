using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator
{
    private Level model;

    public LevelGenerator(Model _model)
    {
        model = _model as Level;
    }

    public void Generate()
    {
        GenerateWavesCount();
        GenerateWave();

    }

    private void  GenerateWavesCount()
    {
        int level = model.level;
        int first = 3;
        List<int> fibanaci = new List<int> ();
        for (int i = 0; i<level+3; i++)
        {
            if (i == 0)
                fibanaci.Add(5);
            else if (i == 1)
                fibanaci.Add(fibanaci[i - 1] + first);
            else
                fibanaci.Add(fibanaci[i - 1] + fibanaci[i - 2]);
        }

        model.waves_cout = fibanaci[level / 2] + level % 2;
        model.waves.Clear();
        for (int i = 0; i < model.waves_cout; i++)
            model.waves.Add(new Wave());

    }

    private void GenerateWave()
    {
        int level = model.level;
        for (int i = 0; i < model.waves_cout; i++)
        {
            

            if (i == 0 && level < 3)
                model.waves[i].type = WaveType.MATRIX;
            else if(i>3 && level >5)
                model.waves[i].type = (WaveType)(Random.Range(0, 3));


            model.waves[i].line_count = (level < 3) ? 5 : Random.Range(5, 8);
            if (model.waves[i].type == WaveType.MATRIX) model.waves[i].line_count = 5;

            model.waves[i].enemies = GenerateEnemyModelsList(model.level, model.waves[i].type);
        }
    }



    private List<EnemyModel> GenerateEnemyModelsList(int level, WaveType type)
    {
        int l = 1;
        if (level > 3 && level < 5)
            l = UnityEngine.Random.Range(1, 3);
        else if (level >= 5)
            l = UnityEngine.Random.Range(1, 4);

        EnemyType e_type = (EnemyType)(l - 1);
        if (type == WaveType.ARMAGEDON)
            e_type = EnemyType.ASTEROID;

        List<EnemyModel> models = new List<EnemyModel>();

        for (int i = 0; i < 5; i++)
        {
            models.Add(new EnemyModel
            {
                live = l,
                enemy_type = e_type,
                speed = 10
            });
        }

        return models;
    }
}
