using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int resources = 0;
    [SerializeField] private GameObject collector = null;
    [SerializeField] private GameObject builder = null;
    [SerializeField] private GameObject farm = null;
    [SerializeField] private GameObject constructionZone = null;
    [SerializeField] private int minionCost = 30;
    [SerializeField] private Color teamColor = Color.white;

    public Color TeamColor { get { return teamColor; } }
    public List<Farm> maintenanceRequests;
    private int zoneCounter;
    private GameObject minionsParent;
    private GameObject buildingsParent;
    private const string cMinionsParentName = "Minions";
    private const string cBuildingsParentName = "Buildings";
    private const string cConstructionZone = "Construction Zone";

    private void Start()
    {
        zoneCounter = 0;
        var spriteRenderer = transform.Find("Base Sprite").gameObject;
        spriteRenderer.GetComponent<SpriteRenderer>().color = teamColor;
        CreateMinionsParentObject();
        CreateBuildingsParentObject();

        DesignateConstructionZone();
    }

    private void Update()
    {
        HandleConstructions();
        HandleMinionSpawning();

        maintenanceRequests.RemoveAll(item => item == null);
    }

    private void HandleMinionSpawning()
    {
        if (resources < minionCost)
        {
            return;
        }

        int numberOfResources = FindObjectsOfType<Resource>().Length;
        int numberOfCollectors = 0;
        foreach (Transform child in minionsParent.transform)
        {
            if (child.GetComponent<Collector>())
            {
                numberOfCollectors++;
            }
        }

        bool areThereMoreResources = numberOfResources > numberOfCollectors;
        bool isBuilderNeeded = IsThereOngoingConstruction() || IsBuildingNeedsMaintenance();
        if (areThereMoreResources || !isBuilderNeeded)
        {
            SpawnCollector();
        }
        else
        {
            SpawnBuilder();
        }
    }

    private void HandleConstructions()
    {
        var constructionZones = GetComponentsInChildren<ConstructionZone>();
        foreach (var constructionZone in constructionZones)
        {
            constructionZone.HandleConstruction(this);
        }
    }

    private bool IsThereOngoingConstruction()
    {
        bool isThereOngoingConstrucion = false;

        var constructionZones = GetComponentsInChildren<ConstructionZone>();
        foreach (var constructionZone in constructionZones)
        {
            isThereOngoingConstrucion |= constructionZone.IsThereOngoingConstruction();
        }

        return isThereOngoingConstrucion;
    }

    public GameObject GetCurrentConstruction()
    {
        GameObject currentConstruction = null;

        var constructionZones = GetComponentsInChildren<ConstructionZone>();
        foreach (var constructionZone in constructionZones)
        {
            currentConstruction = constructionZone.GetCurrentConstruction();
        }

        return currentConstruction;
    }

    private void SpawnCollector()
    {
        var newMinion = Instantiate(collector, transform.position,
                        Quaternion.identity, minionsParent.transform);
        newMinion.GetComponent<Minion>().MyBase = this;

        resources -= minionCost;
    }

    private void SpawnBuilder()
    {
        var newMinion = Instantiate(builder, transform.position,
                        Quaternion.identity, minionsParent.transform);
        newMinion.GetComponent<Minion>().MyBase = this;

        resources -= minionCost;
    }

    private void CreateMinionsParentObject()
    {
        Transform minionsParentTransform = transform.Find(cMinionsParentName);
        if (!minionsParentTransform)
        {
            minionsParent = new GameObject(cMinionsParentName);
            minionsParent.transform.SetParent(transform);
            minionsParent.transform.position = transform.position;
        }
    }

    private void CreateBuildingsParentObject()
    {
        Transform buildingsParentTransform = transform.Find(cBuildingsParentName);
        if (!buildingsParentTransform)
        {
            buildingsParent = new GameObject(cBuildingsParentName);
            buildingsParent.transform.SetParent(transform);
            buildingsParent.transform.position = transform.position;
        }
    }

    private void DesignateConstructionZone()
    {
        bool closerToLeft = transform.position.x <= 0;
        int xPos = closerToLeft ? 500 : -500;
        int yPos = 0;

        const int xGap = 30;
        const int yGap = 30;

        Vector3 displacement = new Vector2(xPos, yPos);
        Vector2 size = new Vector2(400, 300);
        Vector2 gapSize = new Vector2(xGap, yGap);

        zoneCounter++;
        var newZone = Instantiate(constructionZone, transform.position + displacement, 
            Quaternion.identity, buildingsParent.transform);
        newZone.GetComponent<BoxCollider2D>().size = size;
        newZone.GetComponent<ConstructionZone>().ZoneIndex = zoneCounter;
        newZone.GetComponent<ConstructionZone>().GapSize = gapSize;
        newZone.GetComponent<ConstructionZone>().BuildingType = farm;
        newZone.name = cConstructionZone + " " +zoneCounter.ToString();
    }

    internal void SignConstructionComplete(Farm farm)
    {
        var constructionZoneParent = farm.transform.parent;
        var currentConstructionZone = constructionZoneParent.GetComponent<ConstructionZone>();

        if (currentConstructionZone)
        {
            currentConstructionZone.BuildingFinished();
        }
    }

    internal void SignMaintenanceNeeded(Farm building)
    {
        if (!maintenanceRequests.Contains(building))
        { maintenanceRequests.Add(building); }
    }

    internal bool DesignateMaintenance(Builder builder)
    {
        if (maintenanceRequests.Count == 0) { return false; }

        foreach (var target in maintenanceRequests)
        {
            if (!target) { continue; }

            Builder designatedBuilder = FindClosestBuilderTo(target.transform.position);
            if (builder == designatedBuilder && !builder.DesignatedMaintenance)
            {
                builder.DesignatedMaintenance = target.gameObject;
                maintenanceRequests.Remove(target);
                return true;
            }
        }

        return false;
    }

    private Builder FindClosestBuilderTo(Vector3 position)
    {
        Builder designatedBuilder = null;
        float minDistance = float.MaxValue;

        foreach (Transform child in minionsParent.transform)
        {
            var builder = child.GetComponent<Builder>();
            if (builder)
            {
                var distance = Vector3.Distance(child.position, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    designatedBuilder = builder;
                }
            }
        }

        return designatedBuilder;
    }

    internal bool IsBuildingNeedsMaintenance()
    {
        return maintenanceRequests.Count > 0;
    }

    public void TakeResources(int num)
    {
        resources += num;
    }
}
