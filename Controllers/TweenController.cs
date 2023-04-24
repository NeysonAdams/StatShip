using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;


public enum AnimationType
{
    ENEMY_PUSH_MATRIX,
    ENEMY_ATACK,
    ENEMY_LINE
}


public class TweenController
{
    static TweenController instance;
    private PathType path_type = PathType.Linear;
    private List<Tween> tweens = new List<Tween>();
    private List<BulletView> bullets = new List<BulletView>();
    private List<CoinView> bonuses = new List<CoinView>();

    public TweenController()
    {
        if (instance == null)
            instance = this;
    }

    public BulletView AddBullet
    {
        set
        {
            bullets.Add(value);
        }
    }

    public BulletView RemoveBullet
    {
        set
        {
            bullets.Remove(value);
        }
    }

    public CoinView AddBonus
    {
        set
        {
            bonuses.Add(value);
        }
    }

    public CoinView RemoveBonus
    {
        set
        {
            bonuses.Remove(value);
        }
    }

    public static TweenController Instance => instance;

    public void PlayceRotation(Imatrix matrixView)
    {
        Vector3[] path = new Vector3[]{
            new Vector3(0, 2.5f, -3),
            new Vector3(-0.5f, 2, -3),
            new Vector3( 0, 1.5f, -3),
            new Vector3(0.5f,2,-3),
            new Vector3(0,2,-3)
        };

        tweens.Add(matrixView.GetTransform().DOLocalPath(path, 10, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLoops(-1));

    }


    public Tween GetInAnimation (IGOView view, Vector3[] path_coords, bool independ=true, float speed= 2, bool remove_after = false)
    {
        Transform obj = view.GetTransform();
        Tween tween = obj.DOPath(path_coords, speed, PathType.CatmullRom)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .SetLookAt(0.1f);


        if (independ)
        {
            tween.OnComplete(() => {
                RemoveDeadTween(tween);
                if (remove_after)
                    Transform.Destroy(obj.gameObject);
            });
            tweens.Add(tween);
        }
        return tween;
    }

    public Tween PutinMatrix(IGOView view, Imatrix matrix, Vector3[] path_coords, int x, int y)
    {
        Transform obj = view.GetTransform();
        var point = matrix.GetMatrixElement(x, y);
        Tween tween = DOTween.Sequence()
            .Append(GetInAnimation(view, path_coords, false))
            .AppendCallback(() => obj.parent = point)
            .Append(
                obj.DOLocalPath(GenerateFinalPath(obj.localPosition),2, PathType.CatmullRom)
                    .SetSpeedBased()
                    .SetLookAt(0.1f, true)
            );

        tweens.Add(tween);

        tween.OnComplete(()=> {
            RemoveDeadTween(tween);
            });
        return tween;
    }

    private Vector3[] GenerateFinalPath(Vector3 obj)
    {
        Vector3 point = Vector3.zero;
        int kof = point.x > obj.x ? 1 : -1;

        return new Vector3[]
        {
            new Vector3 (Mathf.Abs(point.x - obj.x)/2 * kof, 1.5f, 0),
            new Vector3 (point.x, .5f, 0),
            new Vector3 (0,0,0)
        };
    }

    public Tween AtackAnimation(IGOView view, IPlayer _player, Vector3[] path_coords, Action middleEvent = null)
    {
        Transform obj = view.GetTransform();
        if (obj == null || obj.parent == null)
            return null;
        var p = _player.GetTransform();
        obj.parent = p.parent;
        

        AxisConstraint lock_z = AxisConstraint.Z;
        float out_point = UnityEngine.Random.Range(-5, 5);
        Tween tween = DOTween.Sequence()
            .Append(obj.DOMove(_player.GetTransform().position, 2))
            .Join(obj.DOLookAt(_player.GetTransform().position, 2, lock_z)
            .OnComplete(()=>middleEvent?.Invoke()))
            .Insert(1.5f,obj.DOMove( new Vector3(out_point, -10, _player.GetTransform().position.z), 4))
            .SetEase(Ease.Linear);

        //Tween tween = obj.DOPath(path_coords, 6, path_type).SetLookAt(p.position, true);

        tweens.Add(tween);
        tween.OnComplete(() => {
            Transform.Destroy(obj.gameObject);
            RemoveDeadTween(tween); });

        return tween;
    }

    public Tween ShowMessage(CanvasGroup messageBox, float duration)
    {
        Transform obj = messageBox.transform;
        obj.localScale = new Vector3(1.5f, 1.5f, 1);
        Tween tween =  DOTween.Sequence()
            .Append(obj.DOScale(Vector3.one, 0.2f))
            .Join(messageBox.DOFade(1, 0.2f))
            .AppendInterval(duration)
            .Append(obj.DOScale(new Vector3(0.5f, 0.5f, 1), 0.2f))
            .Join(messageBox.DOFade(0, 0.2f))
            .SetEase(Ease.Linear);

        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);
        return tween;
    }

    public void ShowMessageSequance(string[] messages, TextMeshProUGUI label, CanvasGroup messageBox)
    {
        Sequence tween = DOTween.Sequence();
        for (int i = 0; i < messages.Length; i++)
        {
            int c = i;
            tween.AppendCallback(() => label.text = messages[c])
                .Append(ShowMessage(messageBox, 0.6f));

        }

        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);

    }

    public void FadeIn(CanvasGroup messageBox)
    {
        Tween tween = messageBox.DOFade(1, 1.5f);
        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);

    }

    public void ChangeText(string to, TextMeshProUGUI label)
    {
        Tween tween = label.DOText(to, 0.2f);
        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);
    }

    public void HitAnimation(Material material, int times, Action<bool> doImmortal = null)
    {
        Sequence tween = DOTween.Sequence()
            .AppendCallback(() => doImmortal?.Invoke(true));
        for (int i = 0; i < times; i++) {
            tween.Append(material.DOColor(Color.red, 0.5f))
            .Append(material.DOColor(Color.white, 0.5f));
        }
        tween.AppendCallback(() => doImmortal?.Invoke(false))
            .Play();

        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);
    }

    public void DoSimpleStep(Action prevstep, float waiyInterval, Action poststep, Action<Sequence> update = null)
    {
        Sequence tween = DOTween.Sequence()
            .AppendCallback(() => prevstep?.Invoke())
            .AppendInterval(waiyInterval)
            .AppendCallback(() => poststep?.Invoke());
        tween.OnUpdate(() =>
            {
                update?.Invoke(tween);
            });
        tween.OnKill(() => RemoveDeadTween(tween));
        tweens.Add(tween);
    }

    private void RemoveDeadTween(Tween tween)
    {
        tweens.Remove(tween);
    }

    public void SetPause(bool pause)
    {
        if(pause)
        {
            for (int i = 0; i < tweens.Count; i++)
                tweens[i].Pause();

            for (int i = 0; i < bullets.Count; i++)
                bullets[i].Stop();

            for (int i = 0; i < bonuses.Count; i++)
                bonuses[i].Pause = true;
        }else
        {
            for (int i = 0; i < tweens.Count; i++)
                tweens[i].Play();

            for (int i = 0; i < bullets.Count; i++)
                bullets[i].Move();

            for (int i = 0; i < bonuses.Count; i++)
                bonuses[i].Pause = false;
        }
    }
}



