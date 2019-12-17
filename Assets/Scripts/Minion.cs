using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] protected int maxResourcesToCarry = 15;
    [SerializeField] protected float rangeOfSight = 300.0f;
    protected int resources = 0;

    public Vector3 TargetPosition { get; protected set; }
    public Base MyBase { get; set; }
    public Energy Energy => GetComponent<Energy>();

    protected virtual void Initailze() { }
    protected virtual void UpdateFSM() { }

    void Start()
    {
        if (!MyBase) { MyBase = FindObjectOfType<Base>(); }
        Initailze();
    }

    void Update()
    {
        UpdateFSM();
    }

    public void AddResource(int num)
    {
        resources += num;
    }

    public void TakeDamage(float num)
    {
        Energy.Subtract(num);
    }
}
