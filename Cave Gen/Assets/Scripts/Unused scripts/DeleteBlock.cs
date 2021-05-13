using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteBlock : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
        Debug.Log("delete");
    }
}
