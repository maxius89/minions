using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 200.0f;
    [SerializeField] private float angularVelocity = 500.0f;
    [SerializeField] private int maxResourcesToCarry = 15;
    [SerializeField] private float repulsionForce = 50.0f;
    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy = 100.0f;
    [SerializeField] private float energyDepletionRate = 1.0f;
    [SerializeField] private float rangeOfSight = 300.0f;
    [SerializeField] private float attackRange = 200.0f;
    [SerializeField] GameObject weapon;

    private readonly float distanceThreshold = 10.0f;
    private float velocity;
    private Vector3 targetPosition;
    private Base myBase;
    private int resources = 0;
    private MinionState currentState;

    public enum MinionState
    {
        None,
        Collect,
        ReturnHome,
        Attack
    }

    void Start()
    {
        velocity = maxVelocity;
        if (!myBase) { SetBase(FindObjectOfType<Base>()); }
        energy = maxEnergy;
        currentState = MinionState.Collect;

        FindRandomTargetLocation();
        var arrow = transform.Find("Forward Arrow").gameObject;
        GetComponent<SpriteRenderer>().color = myBase.getTeamColor();
    }

    void Update()
    {
        velocity = maxVelocity * (energy / maxEnergy);

        switch (currentState)
        {
            case MinionState.Collect: UpdateCollectState(); break;
            case MinionState.ReturnHome: UpdateReturnHome(); break;
            case MinionState.Attack: UpdateAttack(); break;
        }

        HandleEnergy();
        SetArrowSpriteColor();
    }

    private void UpdateAttack()
    {
        FindClosestEnemy();
        MoveTowardTarget();


        if (!transform.Find("Weapon"))
        {
            Vector3 weaponOffset = new Vector3(0, 2.6f);
            var newWeapon = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
            newWeapon.transform.SetParent(transform);
            newWeapon.transform.localPosition = weaponOffset;
            newWeapon.transform.localRotation = Quaternion.identity;
            newWeapon.name = "Weapon";
            newWeapon.GetComponent<SpriteRenderer>().color = Color.red;
        }

        if (!AssessEnemies())
        {
            Destroy(transform.Find("Weapon").gameObject);
            currentState = MinionState.Collect;
        }
    }

    private void FindClosestEnemy()
    {
        var minions = FindObjectsOfType<Minion>();
        if (minions.Length == 0) return;

        float minDistance = float.MaxValue;
        foreach (var minion in minions)
        {
            float distance = Vector2.Distance(transform.position, minion.transform.position);
            if (minion.GetBase() != myBase && minDistance > distance && distance <= attackRange)
            {
                minDistance = distance;
                targetPosition = minion.transform.position;
            }
        }
    }

    private void UpdateCollectState()
    {
        SearchForResource();
        MoveTowardTarget();

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
        
        return enemyCnt != 0 && enemyCnt * 2 < friendCnt - 1;
    }

    private void UpdateReturnHome()
    {
        GoBackToBase();
        MoveTowardTarget();

        if (Vector2.Distance(transform.position, myBase.transform.position) < distanceThreshold)
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

    private void SearchForResource()
    {
        Resource[] resources = FindObjectsOfType<Resource>();
        if (resources.Length == 0) return;

        float minDistance = float.MaxValue;
        foreach (Resource resource in resources)
        {
            float distance = Vector2.Distance(transform.position, resource.transform.position);
            if (minDistance > distance && distance <= rangeOfSight)
            {
                minDistance = distance;
                targetPosition = resource.transform.position;
            }
        }
    }

    private void MoveTowardTarget()
    {
        if (Vector2.Distance(targetPosition, transform.position) > distanceThreshold)
        {
            Vector3 moveDirection = transform.position - targetPosition;
            float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg) + 90.0f;
            float fwdStep = velocity * Time.deltaTime; 
            float rotStep = angularVelocity * Time.deltaTime; 

            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.AngleAxis(angle, Vector3.forward), rotStep);
            transform.position += transform.up * fwdStep + Separate();
        }
        else
        {
            FindRandomTargetLocation();
        }
    }

    private Vector3 Separate()
    {
        Vector3 separation = Vector3.zero;
        var minions = FindObjectsOfType<Minion>();

        if (minions.Length > 0)
        {
            foreach (Minion minion in minions)
            {
               Vector3 relativePosition = transform.position - minion.gameObject.transform.position;
               if (relativePosition.sqrMagnitude > Mathf.Epsilon)
               {
                    separation += relativePosition / (relativePosition.sqrMagnitude);
               }
            }
        }

        return separation * repulsionForce;
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
