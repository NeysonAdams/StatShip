using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JoystickView : MonoBehaviour
{
    [SerializeField] private Transform blob;

    private Vector3 m_movement = new Vector2(0,0);
    private float radius = 80;

    private void FixedUpdate()
    {
        blob.localPosition = m_movement;
    }

    public Vector3 Movement
    {
        set
        {
            Vector3 new_position = (-1) * (transform.localPosition - value);
            m_movement = Vector3.ClampMagnitude(new_position, radius);
        }

        get => m_movement;
    }
}
