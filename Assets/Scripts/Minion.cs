using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] private float velocity = 200.0f;
    [SerializeField] private float angularVelocity = 500.0f;
    [SerializeField] private int maxResourcesToCarry = 15;
    private Vector3 targetPosition;
    private readonly float distanceThreshold = 10.0f;
    private Base myBase;
    private int resources = 0;

    void Start()
    {
        FindRandomTargetLocation();
        setBase(FindObjectOfType<Base>());
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
            transform.position += transform.up * fwdStep;
        }
        else
        {
            FindRandomTargetLocation();
        }
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
