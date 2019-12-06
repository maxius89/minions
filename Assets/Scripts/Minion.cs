using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] private int maxResourcesToCarry = 15;
    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy = 100.0f;
    [SerializeField] private float energyDepletionRate = 1.0f;
    [SerializeField] private float rangeOfSight = 300.0f;
    [SerializeField] private float attackRange = 200.0f;
    [SerializeField] GameObject weapon;

    private Vector3 targetPosition;
    private Base myBase;
    private int resources = 0;
    private MinionState currentState;

    public enum MinionState
    {
        None,
        Collect,
        ReturnHome,
        Attack,
        Wander
    }

    void Start()
    {
        if (!myBase) { SetBase(FindObjectOfType<Base>()); }

        energy = maxEnergy;
        currentState = MinionState.Collect;

        FindRandomTargetLocation();
        var arrow = transform.Find("Forward Arrow").gameObject;
        GetComponent<SpriteRenderer>().color = myBase.getTeamColor();
    }

    void Update()
    {
        switch (currentState)
        {
            case MinionState.Collect: UpdateCollectState(); break;
            case MinionState.ReturnHome: UpdateReturnHome(); break;
            case MinionState.Attack: UpdateAttack(); break;
            case MinionState.Wander: UpdateWander(); break;
        }

        HandleEnergy();
        SetArrowSpriteColor();
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
            if (minion.GetBase() != myBase && distance < minDistance)
            {
                minDistance = distance;
                targetPosition = minion.transform.position;
            }
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

    private bool AssessEnemies()
    {
        int enemyCnt = 0, friendCnt = 0;
        var minions = FindObjectsOfType<Minion>();
        foreach (var minion in minions)
        {
            if (Vector2.Distance(transform.position, minion.transform.position) <= attackRange)
            {
               if( minion.GetBase() == myBase) { friendCnt++; }
               else { enemyCnt++; }
            }
        }

        bool majorityAssured = enemyCnt * 2 < friendCnt - 1;
        return enemyCnt != 0 && majorityAssured;
    }

    private void UpdateReturnHome()
    {
        GoBackToBase();

        if (resources <= 0)
        {
            currentState = MinionState.Collect;
        }
    }

    private void HandleEnergy()
    {
        energy -= energyDepletionRate * Time.deltaTime;
        if (energy <= 0) { Destroy(gameObject); }
    }

    private void SetArrowSpriteColor()
    {
        float red = 1 - energy / maxEnergy;
        float green = 1 - energy / maxEnergy;
        float blue = 1 - energy / maxEnergy;
        float alpha = energy / maxEnergy;
        Color spriteColor = new Color(red, green, blue, alpha);
        var arrow = transform.Find("Forward Arrow").gameObject;
        arrow.GetComponent<SpriteRenderer>().color = spriteColor;
    }

    private void GoBackToBase()
    {
        targetPosition = myBase.transform.position;
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
                targetPosition = resource.transform.position;
                foundNewTarget = true;
            }
        }

        return foundNewTarget;
    }

    private void FindRandomTargetLocation()
    {
        int xLim = Screen.currentResolution.width / 2;
        int yLim = Screen.currentResolution.height / 2;

        targetPosition = new Vector2(UnityEngine.Random.Range(-xLim, xLim), UnityEngine.Random.Range(-yLim, yLim));
    }

    public void AddResource(int num)
    {
        resources += num;
    }

    public void TakeDamage(float num)
    {
        energy -= num;
    }

    public void SetBase(Base newBase)
    {
        myBase = newBase;
    }

    public Base GetBase()
    {
        return myBase;
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    public float GetEnergyCoeff()
    {
        return energy / maxEnergy;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<Base>() == myBase)
        {
            myBase.TakeResources(resources);
            resources = 0;
            energy = maxEnergy;
        }
    }
}
