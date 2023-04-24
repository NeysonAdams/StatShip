using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class EnemyView : MonoBehaviour, IEnemy
{
    [SerializeField] BulletView bullet;
    [SerializeField] Transform explosion;
    [SerializeField] MeshRenderer renderer;

    private EnemyModel model;
    private Tween current_tween;
    private bool can_shoot = true;
    private ISoundHolder sounds;

    public event Action<IEnemy> movemetFinish;
    public event Action<IEnemy> destroy;
    public event Action<int> scoreUp;
    public event Action<Vector3> bonusEvent;

    public void SetModel(Model _model)
    {
        model = _model as EnemyModel;
        gameObject.name = "Enemy";
        if (model.enemy_type == EnemyType.ASTEROID)
        {
            model.live = 20;
            transform.position = new Vector3(transform.position.x,
                transform.position.y + UnityEngine.Random.Range(-0.5f, 0.5f),
                transform.position.z);
        }
        else if (model.enemy_type == EnemyType.WITHSHILD)
        {
            model.live = 2;
        }
        else if (model.enemy_type == EnemyType.IMMORTAL)
        {
            model.live = 6;
        }
        else
            model.live = 0;

    }

    public Model GetModel()
    {
        return model;
    }

    public void Shoot()
    {
        if (can_shoot)
        {
            var q = transform.rotation.eulerAngles;
            BulletView m_bullet = (BulletView)Instantiate(bullet, transform.position,
                Quaternion.Euler(q.x, -180, q.z));
            m_bullet.gameObject.name = "EnemyBolt";
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void Move(object[] args)
    {
        if (args.Length == 0)
            return;
        AnimationType type = (AnimationType)args[0];

        switch(type)
        {
            case AnimationType.ENEMY_PUSH_MATRIX:
                TweenController.Instance.PutinMatrix(this, (Imatrix)args[1], (Vector3[])args[2], (int)args[3], (int)args[4]);
                break;
            case AnimationType.ENEMY_ATACK:
                TweenController.Instance.AtackAnimation(this, (IPlayer)args[1], (Vector3[])args[2], ()=> can_shoot = false);
                break;
            case AnimationType.ENEMY_LINE:
                TweenController.Instance.GetInAnimation(this, (Vector3[])args[2], true, 4, true);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name.Equals("Bolt(Clone)") ||
            other.gameObject.name.Equals("Player"))
        {
            
            if(other.gameObject.name.Equals("Bolt(Clone)"))
                Destroy(other.gameObject);
            if (model.live > 0)
            {
                model.live--;
                TweenController.Instance.HitAnimation(renderer.materials[0], 1);
            }
            else
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                scoreUp?.Invoke(5);
                bonusEvent?.Invoke(transform.position);
                if (model.enemy_type == EnemyType.ASTEROID)
                    sounds.PlaySFx(SoundsSFX.EXPLOSION_ASTEROID);
                else
                    sounds.PlaySFx(SoundsSFX.EXPLOSION_ENEMY);
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {

        destroy?.Invoke(this);
        movemetFinish = null;
        destroy = null;
        scoreUp = null;
        bonusEvent = null;
    }

    public void SetSounds(ISoundHolder holder)
    {
        sounds = holder;
    }
}

