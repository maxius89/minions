using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private int value = 3;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Destroy(gameObject);

        var minion = otherCollider.GetComponent<Minion>();
        if (minion)
        {
            minion.addResource(value);
        }
    }
}
