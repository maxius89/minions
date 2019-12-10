using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] protected int maxResourcesToCarry = 15;
    [SerializeField] protected float energy;
    [SerializeField] protected float maxEnergy = 100.0f;
    [SerializeField] protected float energyDepletionRate = 1.0f;
    [SerializeField] protected float rangeOfSight = 300.0f;
    protected int resources = 0;

    public Vector3 TargetPosition { get; protected set; }
    public Base MyBase { get; set; }
    
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
        HandleEnergy();
    }

    private void HandleEnergy()
    {
        energy -= energyDepletionRate * Time.deltaTime;
        if (energy <= 0) { Destroy(gameObject); }
    }

    public void AddResource(int num)
    {
        resources += num;
    }

    public void TakeDamage(float num)
    {
        energy -= num;
    }

    public float GetEnergyCoeff()
    {
        return energy / maxEnergy;
    }
}
