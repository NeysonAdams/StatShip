using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class PlayerViev : MonoBehaviour, IPlayer
{
    [SerializeField] Rigidbody rdbody;
    [SerializeField] MeshRenderer render;
    [SerializeField] BulletView bullet;
    [SerializeField] Transform explosion;

    private PlayerModel playerModel;
    private Boundery boundery = new Boundery();
    private Vector3 movement = new Vector3(0,0);
    private float time = 0;
    private bool pause = false;
    private ISoundHolder sounds;
    private bool canShoot = false;

    public event Action<Transform> destroy;
    public event Action addCoin;
    public event Action<int> setLives;

    public void PutonStartPoint()
    {
        transform.position = new Vector3(0, -4, 92);
        canShoot = true;
    }

    private void Update()
    {
        if (this.playerModel == null)
            return;
        if (!pause)
        {
            time += Time.deltaTime;
            if (time > playerModel.shoot_timing)
            {
                Shoot();
                time = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        if (this.playerModel == null)
            return;
        rdbody.velocity = movement * this.playerModel.speed;

        rdbody.position = new Vector3(
            Mathf.Clamp(rdbody.position.x, boundery.xMin, boundery.xMax),
            Mathf.Clamp(rdbody.position.y, boundery.yMin, boundery.yMax),
            rdbody.position.z
            );

        rdbody.rotation = Quaternion.Euler(-90, 0, rdbody.velocity.x * -playerModel.tilt);
    }

    public Model GetModel()
    {
        return playerModel;
    }

    public void Shoot()
    {

        if (!canShoot)
            return;
        BulletView bolt, bolt1, bolt2, bolt3, bolt4;
        switch (playerModel.atack_type)
        {
            case AtackType.SINGLE:
                bolt = (BulletView)Instantiate(bullet, transform.parent);
                bolt.transform.position = transform.position;
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                break;

            case AtackType.DOUBLE:
                bolt = (BulletView)Instantiate(bullet, transform.parent);
                bolt.transform.position = transform.position - new Vector3(0.1f, 0, 0);
                bolt1 = (BulletView)Instantiate(bullet, transform.parent);
                bolt1.transform.position = transform.position + new Vector3(0.1f, 0, 0);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                break;
            case AtackType.TRPPLE:
                bolt = (BulletView)Instantiate(bullet, transform.parent);
                bolt.transform.position = transform.position;
                bolt1 = (BulletView)Instantiate(bullet, transform.parent);
                bolt1.transform.position = transform.position - new Vector3(0.2f, 0, 0);
                bolt2 = (BulletView)Instantiate(bullet, transform.parent);
                bolt2.transform.position = transform.position + new Vector3(0.2f, 0, 0);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                break;
            case AtackType.REDIAL:
                bolt1 = (BulletView)Instantiate(bullet, transform.parent);
                bolt1.transform.position = transform.position;
                bolt2 = (BulletView)Instantiate(bullet, transform.parent);
                bolt2.transform.position = transform.position;
                bolt2.transform.rotation = Quaternion.Euler(-60, -90, 90);
                bolt3 = (BulletView)Instantiate(bullet, transform.parent);
                bolt3.transform.position = transform.position;
                bolt3.transform.rotation = Quaternion.Euler(-30, -90, 90);
                bolt4 = (BulletView)Instantiate(bullet, transform.parent);
                bolt4.transform.position = transform.position;
                bolt4.transform.rotation = Quaternion.Euler(-120, -90, 90);
                bolt = (BulletView)Instantiate(bullet, transform.parent);
                bolt.transform.position = transform.position;
                bolt.transform.rotation = Quaternion.Euler(-150, -90, 90);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                sounds.PlaySFx(SoundsSFX.WEPON_PLAYER);
                break;
        }

    }

    public void Move(object[] args)
    {
        movement = (Vector2)args[0] ;
    }

    public void SetModel(Model model)
    {
        this.playerModel = model as PlayerModel;
    }


    public Transform GetTransform()
    {
        return transform;
    }

    public void Move(Tween movment)
    {
        throw new System.NotImplementedException();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Enemy") ||
            other.gameObject.name.Equals("EnemyBolt"))
        {
            Destroy(other.gameObject);
            
            playerModel.lives--;
            setLives?.Invoke(playerModel.lives);
            if (playerModel.lives == 0)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                destroy?.Invoke(transform);
                Destroy(gameObject);
                sounds.PlaySFx(SoundsSFX.EXPLOSION_PLAYER);
            }
            else
            {
                var materials = render.materials;
                TweenController.Instance.HitAnimation(materials[0], 3);
            }
        }

        if(other.gameObject.name.Equals("Coin(Clone)"))
        {
            Destroy(other.gameObject);
            playerModel.coins ++;
            addCoin?.Invoke();
            sounds.PlaySFx(SoundsSFX.BONUS);
        }

        if (other.gameObject.name.Equals("Sword(Clone)"))
        {
            Destroy(other.gameObject);
            sounds.PlaySFx(SoundsSFX.BONUS);
            int a_type = (int)playerModel.atack_type;
            if(a_type <=3)
                playerModel.atack_type = (AtackType)(a_type + 1);
        }

        if (other.gameObject.name.Equals("Star(Clone)"))
        {
            Destroy(other.gameObject);
            sounds.PlaySFx(SoundsSFX.BONUS);
            playerModel.shoot_timing /=2;
        }

        if (other.gameObject.name.Equals("Shild(Clone)"))
        {
            Destroy(other.gameObject);
            playerModel.lives++;
            setLives?.Invoke(playerModel.lives);
            sounds.PlaySFx(SoundsSFX.BONUS);
        }
    }

    public void AddScore(int score)
    {
        playerModel.score += score;
    }

    private void OnDestroy()
    {
        destroy = null;
        setLives = null;
        addCoin = null;
    }

    public void SetSounds(ISoundHolder holder)
    {
        sounds = holder;
    }

    public void SetPause(bool _pause)
    {
        pause = _pause;
    }
}
