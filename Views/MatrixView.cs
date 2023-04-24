using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MatrixView : MonoBehaviour, Imatrix
{
    [SerializeField]
    private List<Transform> elements = new List<Transform>();

    private Tween currentTween;

    public void StartMatrixAnimation()
    {
        TweenController.Instance.PlayceRotation(this);
    }

    public Transform GetMatrixElement(int x, int y)
    {
        return elements[x*5+y];
    }

    public Vector3 GetMatrixPosition(int x, int y)
    {
        return elements[x * 5 + y].position;
    }

    public Transform GetPrarent()
    {
        return transform.parent;
    }


    public Transform GetTransform()
    {
        return transform;
    }
    public void SetTween(Tween tween)
    {
        currentTween = tween;
    }

}
