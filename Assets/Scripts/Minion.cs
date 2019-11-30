using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] private float velocity = 200.0f;
    [SerializeField] private float angularVelocity = 500.0f;
    [SerializeField] private int maxResourcesToCarry = 15;
    [SerializeField] private float repulsionForce = 50.0f;
    [SerializeField] private float energy;
    [SerializeField] private float maxEnergy = 100.0f;
    [SerializeField] private float energyDepletionRate = 1.0f;

    private readonly float distanceThreshold = 10.0f;
    private Vector3 targetPosition;
    private Base myBase;
    private int resources = 0;

    void Start()
    {
        FindRandomTargetLocation();
        if (!myBase) { setBase(FindObjectOfType<Base>()); }
        energy = maxEnergy;

        var arrow = transform.Find("Forward Arrow").gameObject;
        GetComponent<SpriteRenderer>().color = myBase.getTeamColor();
    }

    void Update()
    {
        if (resources >= maxResourcesToCarry)
        {
            GoBackToBase();
        }
        else
        {
            SearchForResource();
        }
        
        MoveTowardTarget();
        HandleEnergy();
        SetArrowSpriteColor();
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
            if (minDistance > distance)
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

    public void addResource(int num)
    {
        resources += num;
    }

    internal void addEnergy(float num)
    {
        energy += num;
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    public void setBase(Base newBase)
    {
        myBase = newBase;
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.GetComponent<Base>() == myBase)
        {
            myBase.TakeResources(resources);
            resources = 0;
        }
    }
}
