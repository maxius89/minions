﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float damage = 100;
    private Minion parent;

    private void Start()
    {
        var parentTransform = transform.parent;
        parent = parentTransform.GetComponent<Minion>();
    }

    private void OnCollisionEnter2D(Collision2D otherCollider)
    {
        var otherMinion = otherCollider.gameObject.GetComponent<Minion>();
        if (!otherMinion) return;

        if (otherMinion.GetBase() != parent.GetBase())
        {
            otherMinion.TakeDamage(damage);
        }
    }

}
