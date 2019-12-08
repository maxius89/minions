using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    [SerializeField] private int costOfConstruction;
    [SerializeField] private float timeBetweenSpawnsInSeconds;
    [SerializeField] GameObject resource;

    public Base MyBase { get; set; }
    public bool IsBuilingComplete { get; private set; }
    GameObject currentResource;
    private float timer;
    private int constructionProgress;

    void Start()
    {
        IsBuilingComplete = false;
        GetComponent<SpriteRenderer>().color = MyBase.TeamColor;
        constructionProgress = 0;
        timer = 0;
    }

    void Update()
    {
        if (!IsBuilingComplete)
        {
            if (constructionProgress >= costOfConstruction)
            {
                IsBuilingComplete = true;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        else if (!currentResource)
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
