using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    [SerializeField] private float speed = 200.0f;

    private Vector2 targetPosition;
    private readonly float distanceThreshold = 10.0f;

    void Start()
    {
        FindRandomTargetLocation();
    }

    void Update()
    {
        SearchForResource();
        MoveTowardTarget();
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
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
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
}
