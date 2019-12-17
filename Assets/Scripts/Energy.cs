using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField] protected float energy;
    [SerializeField] protected float maxEnergy = 100.0f;
    [SerializeField] protected float energyDepletionRate = 1.0f;

    public float EnergyCoeff => energy / maxEnergy;

    void Start()
    {
        FillToMaximum();
    }

    void Update()
    {
        UpdateEnergy();
    }

    private void UpdateEnergy()
    {
        energy -= energyDepletionRate * Time.deltaTime;
        if (energy <= 0) { Destroy(gameObject); }
    }

    public void FillToMaximum()
    {
        energy = maxEnergy;
    }

    public void Subtract(float num)
    {
        energy -= num;
    }
}
