using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinView : MonoBehaviour
{
    [SerializeField] Transform coin;
    private bool pause = false;
    // Start is called before the first frame update
    void Start()
    {
        TweenController.Instance.AddBonus = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause)
        {
            coin.rotation = Quaternion.Euler(0, coin.rotation.eulerAngles.y + Time.deltaTime * 60, 0);
            transform.position += Vector3.down * 0.005f;
        }
    }

    public bool Pause
    {
        set
        {
            pause = value;
        }
    }

    private void OnDestroy()
    {
        TweenController.Instance.RemoveBonus = this;
    }
}
