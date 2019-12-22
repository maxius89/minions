using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private int value = 3;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        var collector = otherCollider.GetComponent<Collector>();
        if (collector)
        {
            collector.AddResource(value);
            Destroy(gameObject);
        }
    }
}
