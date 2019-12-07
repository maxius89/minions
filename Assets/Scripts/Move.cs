using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float maxVelocity = 500.0f;
    [SerializeField] private float angularVelocity = 500.0f;
    [SerializeField] private float repulsionForce = 50.0f;

    private Vector3 targetPosition;
    private float velocity;
    private readonly float distanceThreshold = 10.0f;

    void Start()
    {
        velocity = maxVelocity;
        targetPosition = GetComponent<Minion>().TargetPosition;
    }

    void Update()
    {
        var minion = GetComponent<Minion>();
        velocity = maxVelocity * minion.GetEnergyCoeff();
        targetPosition = minion.TargetPosition;
        MoveTowardTarget();
    }

    private void MoveTowardTarget()
    {
        Vector3 moveDirection = transform.position - targetPosition;
        float angle = (Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg) + 90.0f;
        float fwdStep = velocity * Time.deltaTime;
        float rotStep = angularVelocity * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.AngleAxis(angle, Vector3.forward), rotStep);
        transform.position += transform.up * fwdStep + Separate();
    }

    private Vector3 Separate()
    { 
        var minions = FindObjectsOfType<Minion>();
        if (minions.Length == 0) { return Vector3.zero; }

        Vector3 separation = Vector3.zero;
        foreach (var minion in minions)
        {
            Vector3 relativePosition = transform.position - minion.gameObject.transform.position;
            if (relativePosition.sqrMagnitude > Mathf.Epsilon)
            {
                separation += relativePosition / (relativePosition.sqrMagnitude);
            }
        }

        return separation * repulsionForce;
    }
}
