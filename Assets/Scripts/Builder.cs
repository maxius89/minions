using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : Minion
{
    protected BuilderState currentState;
    private GameObject designatedBuilding;
    public bool IsCarryingMaterial { get; set; }
    public enum BuilderState
    {
        None,
        Build,
        Maintain,
        ReturnHome,
        Wander
    }

    protected override void Initailze()
    {
        energy = maxEnergy;
        currentState = BuilderState.Wander;

        GetComponent<SpriteRenderer>().color = MyBase.TeamColor;
    }

    protected override void UpdateFSM()
    {
        switch (currentState)
        {
            case BuilderState.Build: UpdateBuild(); break;
            case BuilderState.Maintain: UpdateMaintain(); break;
            case BuilderState.ReturnHome: UpdateReturnHome(); break;
            case BuilderState.Wander: UpdateWander(); break;
        }
    }

    private void UpdateWander()
    {
        FindRandomTargetLocation();

        if (IsConstructionAvailable())
        {
            currentState = BuilderState.Build;
        }
        //else if (IsBuildingNeedsMaintianance())
        //{
        //    currentState = BuilderState.Maintain;
        //
        //}

    }

    private bool IsBuildingNeedsMaintianance()
    {
        throw new NotImplementedException();
    }

    private bool IsConstructionAvailable() => MyBase.GetCurrentConstruction();

    private void UpdateReturnHome()
    {
        throw new NotImplementedException();
    }

    private void UpdateMaintain()
    {
        throw new NotImplementedException();
    }

    private void UpdateBuild()
    {
       designatedBuilding = MyBase.GetCurrentConstruction();

        if(IsCarryingMaterial && designatedBuilding)
        {
            TargetPosition = designatedBuilding.transform.position;
        }
        else
        {
            TargetPosition = MyBase.transform.position;
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
            energy = maxEnergy;

            if (designatedBuilding)
            {
                IsCarryingMaterial = true;
            }
        }
        else if (otherCollider.gameObject == designatedBuilding)
        {
            designatedBuilding.GetComponent<Farm>().TakeConstructionMaterial();
            IsCarryingMaterial = false;
        }
    }
}
