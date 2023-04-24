using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutofBounderry : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name.Equals("Enemy"))
            Debug.Log(other.gameObject.name);

        Destroy(other.gameObject);

    }

}
