using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    [SerializeField] private int costOfConstruction = 0;
    [SerializeField] private float timeBetweenSpawnsInSeconds = 0.0f;
    [SerializeField] private GameObject resource = null;

    public Base MyBase { get; private set; }
    public Energy Energy => GetComponent<Energy>();
    public bool IsBuilingComplete { get; private set; }
    private GameObject currentResource;
    private float timer;
    private int constructionProgress;

    private GameObject resourceParent;
    private const string cResourceParentName = "Resources";

    void Start()
    {
        MyBase = GetComponentInParent<Base>();

        IsBuilingComplete = false;
        GetComponent<SpriteRenderer>().color =  SetStartingColor();
        constructionProgress = 0;
        timer = 0;

        CreateResourceParent();
    }

    void Update()
    {
        if (IsBuilingComplete)
        {
            CheckEnergyState();

            if (currentResource) { return; }
            GenerateResource();
        }
        else
        {
            CheckConstructionState();
        }
    }

    private void CheckEnergyState()
    {
        if (Energy.EnergyCoeff < 0.3)
        {
            MyBase.SignMaintenanceNeeded(this);
        }
    }

    private void CheckConstructionState()
    {
        if (constructionProgress < costOfConstruction) { return; }

        IsBuilingComplete = true;
        GetComponent<SpriteRenderer>().color = MyBase.TeamColor;
    }

    private void GenerateResource()
    {        
        timer += Time.deltaTime;
        if (timer >= timeBetweenSpawnsInSeconds)
        {
            var zDisplacement = new Vector3(0, 0, -1);
            currentResource = Instantiate(resource, transform.position + zDisplacement, 
                transform.rotation, resourceParent.transform);
            timer = 0;
        }
    }

    public void TakeConstructionMaterial()
    {
        constructionProgress++;
    }

    private void CreateResourceParent()
    {
        resourceParent = GameObject.Find(cResourceParentName);
        if (!resourceParent)
        {
            resourceParent = new GameObject(cResourceParentName);
        }
    }

    private Color SetStartingColor()
    {
        Color baseColor = MyBase.TeamColor;
        var red = baseColor.r * 0.75f;
        var green = baseColor.g * 0.75f;
        var blue = baseColor.b * 0.75f;
        var alpha = 0.6f;

        return new Color(red, green, blue, alpha);
    }
}