using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidView : MonoBehaviour
{
    private float x_angle, y_angle, z_angle;
    private void Start()
    {
        x_angle = Random.Range(-5, 5);
        y_angle = Random.Range(-5, 5);
        z_angle = Random.Range(-5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x + x_angle,
            transform.rotation.eulerAngles.y + y_angle,
            transform.rotation.eulerAngles.z + z_angle
            );
    }
}
