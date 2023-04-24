using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum BulletType
{
    PLAYER,
    ENEMY
}

public class BulletView : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] BulletType type;
    // Start is called before the first frame update
    void Start()
    {
        TweenController.Instance.AddBullet = this;
        Move();
    }

    public void Move()
    {
        if (type == BulletType.PLAYER)
            rigidbody.velocity = transform.forward * 20;
        else
            rigidbody.velocity = transform.forward * 3;
    }

    public void Stop()
    {
        rigidbody.velocity = Vector3.zero;
    }

    private void OnDestroy()
    {
        TweenController.Instance.RemoveBullet = this;
    }

}
