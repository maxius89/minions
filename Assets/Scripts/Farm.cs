using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawnsInSeconds;
    [SerializeField] GameObject resource;
    GameObject currentResource;
    private float timer;

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        if (!currentResource)
        {
            GenerateResource();
        }
    }

    private void GenerateResource()
    {        
        timer += Time.deltaTime;
        if (timer >= timeBetweenSpawnsInSeconds)
        {
            currentResource = Instantiate(resource, transform.position, transform.rotation);
            timer = 0;
        }
    }
}
