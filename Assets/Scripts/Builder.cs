using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Minion
{
    [SerializeField] private float repairValue = 0;
    [SerializeField] protected BuilderState currentState;

    public GameObject DesignatedConstruction { get; set; }
    public GameObject DesignatedMaintenance { get; set; }
    public bool IsCarryingMaterial { get; set; }
    public float idleCounter;
    public enum BuilderState
    {
        None,
        Build,
        Maintain,
        SelfRecycle,
        Wander
    }

    protected override void Initailze()
    {
        idleCounter = 0;
        currentState = BuilderState.Wander;

        GetComponent<SpriteRenderer>().color = MyBase.TeamColor;
    }

    protected override void UpdateFSM()
    {
        switch (currentState)
        {
            case BuilderState.Build: UpdateBuild(); break;
            case BuilderState.Maintain: UpdateMaintain(); break;
            case BuilderState.SelfRecycle: UpdateSelfRecycle(); break;
            case BuilderState.Wander: UpdateWander(); break;
        }
    }

    private void UpdateWander()
    {
        FindRandomTargetLocation();
        idleCounter += Time.deltaTime;

        if (idleCounter > 5.0f || Energy.EnergyCoeff < 0.6f)
        {
            currentState = BuilderState.SelfRecycle;
        }
        else if (MyBase.DesignateMaintenance(this))
        {
            currentState = BuilderState.Maintain;
            idleCounter = 0;
        }
        else if (IsConstructionAvailable())
        {
            currentState = BuilderState.Build;
            idleCounter = 0;
        }       
    }

    private bool IsConstructionAvailable() => MyBase.GetCurrentConstruction();

    private void UpdateSelfRecycle()
    {
        TargetPosition = MyBase.transform.position;
    }

    private void UpdateMaintain()
    {
        bool isRepaired = true;
        if (DesignatedMaintenance)
        {
            TargetPosition = DesignatedMaintenance.transform.position;
            isRepaired = DesignatedMaintenance.GetComponent<Energy>().EnergyCoeff > 0.6;
        }
 
        if (isRepaired && IsConstructionAvailable())
        {
            DesignatedMaintenance = null;
            currentState = BuilderState.Build;
        }
        else if (isRepaired)
        {
            DesignatedMaintenance = null;
            currentState = BuilderState.Wander;
        }
    }

    private void UpdateBuild()
    {
       DesignatedConstruction = MyBase.GetCurrentConstruction();

        if(IsCarryingMaterial && DesignatedConstruction)
        {
            TargetPosition = DesignatedConstruction.transform.position;
        }
        else
        {
            TargetPosition = MyBase.transform.position;
        }

        if (MyBase.DesignateMaintenance(this))
        {
            DesignatedConstruction = null;
            currentState = BuilderState.Maintain;
        }
        else if (!IsConstructionAvailable())
        {
            DesignatedConstruction = null;
            currentState = BuilderState.Wander;
        }
    }

    private void FindRandomTargetLocation()
    {
        int xLim = Screen.currentResolution.width / 2;
        int yLim = Screen.currentResolution.height / 2;

        TargetPosition = new Vector2(UnityEngine.Random.Range(-xLim, xLim), UnityEngine.Random.Range(-yLim, yLim));
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<Base>() == MyBase)
        {
            if (currentState == BuilderState.SelfRecycle)
            {
                MyBase.TakeResources(20);
                Destroy(gameObject);
            }

            Energy.FillToMaximum();

            if (DesignatedConstruction)
            {
                IsCarryingMaterial = true;
            }
        }
        else if (otherCollider.gameObject == DesignatedConstruction && currentState == BuilderState.Build)
        {
            DesignatedConstruction.GetComponent<Farm>().TakeConstructionMaterial();
            IsCarryingMaterial = false;
        }
        else if (otherCollider.gameObject == DesignatedMaintenance && currentState == BuilderState.Maintain)
        {
            DesignatedMaintenance.GetComponent<Energy>().Add(repairValue);
        }
    }
}
