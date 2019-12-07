﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : Minion
{
    protected MinionState currentState;

    public enum MinionState
    {
        None,
        Collect,
        ReturnHome,
        Attack,
        Wander
    }

    protected override void Initailze()
    {
        energy = maxEnergy;
        currentState = MinionState.Collect;

        FindRandomTargetLocation();
        GetComponent<SpriteRenderer>().color = MyBase.getTeamColor();
    }

    protected override void UpdateFSM()
    {
        switch (currentState)
        {
            case MinionState.Collect: UpdateCollectState(); break;
            case MinionState.ReturnHome: UpdateReturnHome(); break;
            case MinionState.Attack: UpdateAttack(); break;
            case MinionState.Wander: UpdateWander(); break;
        }
    }

    private void UpdateCollectState()
    {
        if (!SearchForResource())
        {
            currentState = MinionState.Wander;
        }

        if (AssessEnemies())
        {
            currentState = MinionState.Attack;
        }
        else if (resources >= maxResourcesToCarry)
        {
            currentState = MinionState.ReturnHome;
        }
    }

    private void UpdateReturnHome()
    {
        GoBackToBase();

        if (resources <= 0)
        {
            currentState = MinionState.Collect;
        }
    }

    private void UpdateAttack()
    {
        DeployWeapon();
        FindClosestEnemy();

        if (!AssessEnemies())
        {
            Destroy(transform.Find("Weapon").gameObject);
            currentState = MinionState.Collect;
        }
    }

    private void UpdateWander()
    {
        FindRandomTargetLocation();

        if (AssessEnemies())
        {
            currentState = MinionState.Attack;
        }
        else if (SearchForResource())
        {
            currentState = MinionState.Collect;
        }
    }

    private bool AssessEnemies()
    {
        int enemyCnt = 0, friendCnt = 0;
        var minions = FindObjectsOfType<Minion>();
        foreach (var minion in minions)
        {
            if (Vector2.Distance(transform.position, minion.transform.position) <= attackRange)
            {
                if (minion.MyBase == MyBase) { friendCnt++; }
                else { enemyCnt++; }
            }
        }

        bool majorityAssured = enemyCnt * 2 < friendCnt - 1;
        return enemyCnt != 0 && majorityAssured;
    }

    private void DeployWeapon()
    {
        if (transform.Find("Weapon")) { return; }

        Vector3 weaponOffset = new Vector3(0, 2.6f);
        var newWeapon = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
        newWeapon.transform.SetParent(transform);
        newWeapon.transform.localPosition = weaponOffset;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.name = "Weapon";
        newWeapon.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void FindClosestEnemy()
    {
        var minions = FindObjectsOfType<Minion>();
        if (minions.Length == 0) return;

        float minDistance = float.MaxValue;
        foreach (var minion in minions)
        {
            float distance = Vector2.Distance(transform.position, minion.transform.position);
            if (minion.MyBase != MyBase && distance < minDistance)
            {
                minDistance = distance;
                TargetPosition = minion.transform.position;
            }
        }
    }

    private void GoBackToBase()
    {
        TargetPosition = MyBase.transform.position;
    }

    private bool SearchForResource()
    {
        Resource[] resources = FindObjectsOfType<Resource>();
        if (resources.Length == 0) return false;

        bool foundNewTarget = false;
        float minDistance = float.MaxValue;
        foreach (Resource resource in resources)
        {
            float distance = Vector2.Distance(transform.position, resource.transform.position);
            if (minDistance > distance && distance <= rangeOfSight)
            {
                minDistance = distance;
                TargetPosition = resource.transform.position;
                foundNewTarget = true;
            }
        }

        return foundNewTarget;
    }

    private void FindRandomTargetLocation()
    {
        int xLim = Screen.currentResolution.width / 2;
        int yLim = Screen.currentResolution.height / 2;

        TargetPosition = new Vector2(Random.Range(-xLim, xLim), Random.Range(-yLim, yLim));
    }
}
