using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum AtackType
{
    SINGLE,
    DOUBLE,
    TRPPLE,
    REDIAL
}

public enum EnemyType
{
    SIMPLE,
    WITHSHILD,
    KAMICADLZE,
    IMMORTAL,
    ASTEROID
}

public enum WaveType
{
    MATRIX,
    CROSSLINE,
    ARMAGEDON
}
public enum LevelType
{
    LEVELGAME,
    QUICKGAME
}


public abstract class Model
{
    public int id;
}

[Serializable]
public class PlayerModel : Model
{
    public float speed = 10;
    [Range(0.1f, 4)]
    public float shoot_timing = 3;
    public int lives = 3;
    public float tilt = 0;
    public AtackType atack_type = AtackType.SINGLE;
    public int coins = 0;
    public int score = 0;
}

public class EnemyModel : Model
{
    public float speed;
    public int live = 1;
    public EnemyType enemy_type = EnemyType.SIMPLE;

}

[Serializable]
public class Level : Model
{
    public int level = 1;
    public int waves_cout = 2;
    public int current_wave = 1;
    public List<Wave> waves;
    public LevelType lvl_Type = LevelType.LEVELGAME;

}
[Serializable]
public class Wave
{
    public WaveType type = WaveType.MATRIX;
    public int line_count = 3;
    public int currrent_line = 1;
    public List<EnemyModel> enemies;

}

[Serializable]
public class RespownePoints : Model
{
    // Informaton about Enemy Respowne playses
    public Vector3[] respownePoints;
    //Animation Entry path for Enemy 
    public List<Path> in_paths = new List<Path>();

    public List<Path> line_path = new List<Path>();
}

[Serializable]
public struct Path
{
    public Vector3[] path;
}

public class Boundery
{
    public float xMin = -2.5f, xMax = 2.5f, yMin = -4.4f, yMax = 4;
}
[Serializable]
public enum BonusType
{
    NONE,
    COIN,
    SHIELD,
    SWORD,
    STAR
}

[Serializable]
public class BonusProbabilty
{
    public BonusType type = BonusType.NONE;
    [Range(0,100)]
    public float probability_weght;
    public float probability_percent;

    [HideInInspector]
    public float probabilityRangeFrom;
    [HideInInspector]
    public float probabilityRangeTo;
}

[Serializable]
public class BonusModel : Model
{
    public List<Transform> prefabs = new List<Transform>();
    public List<BonusProbabilty> probabilties = new List<BonusProbabilty>();
}





