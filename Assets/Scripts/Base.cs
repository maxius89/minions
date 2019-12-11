using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int resources = 0;
    [SerializeField] GameObject collector;
    [SerializeField] GameObject builder;
    [SerializeField] GameObject farm;
    [SerializeField] GameObject constructionZone;
    [SerializeField] int minionCost = 30;
    [SerializeField] private Color teamColor;

    public Color TeamColor { get { return teamColor; } }
    private GameObject minionsParent;
    private GameObject buildingsParent;
    private const string cMinionsParentName = "Minions";
    private const string cBuildingsParentName = "Buildings";
    private const string cConstructionZone = "Construction Zone";

    private void Start()
    {
        var spriteRenderer = transform.Find("Base Sprite").gameObject;
        spriteRenderer.GetComponent<SpriteRenderer>().color = teamColor;
        CreateMinionsParentObject();
        CreateBuildingsParentObject();

        DesignateConstructionZone();
    }

    private void Update()
    {
        HandleContstrucions();

        // TODO: Create decision making
        if (resources >= minionCost)
        {
            int randomSpawn = UnityEngine.Random.Range(0, 2);
            if (randomSpawn == 0)
            {
                SpawnCollector();
            }
            else
            {
                SpawnBuilder();
            }
        }
    }

    private void HandleContstrucions()
    {
        var currentConstructionZone = transform.Find(cConstructionZone).GetComponent<ConstructionZone>();
        if (!IsThereOngoingConstruction() && !currentConstructionZone.IsZoneFull())
        {
            var newConstructionPosition = SelectNewConstuctionPosition();

            var newFarm = Instantiate(farm, newConstructionPosition,
                Quaternion.identity, buildingsParent.transform);
            newFarm.GetComponent<Farm>().MyBase = this;
        }

    }

    private bool IsThereOngoingConstruction()
    {
        bool isThereOngoingConstrucion = false;
        foreach (Transform child in buildingsParent.transform)
        {
            var farm = child.GetComponent<Farm>();
            if (!farm.IsBuilingComplete)
            {
                isThereOngoingConstrucion = true;
                break;
            }
        }

        return isThereOngoingConstrucion;
    }

    private Vector3 SelectNewConstuctionPosition()
    {
        int indexOfNextBuilding = buildingsParent.transform.childCount;
        const int xGap = 30;
        const int yGap = 30;

        var currentConstructionZone = transform.Find(cConstructionZone).GetComponent<ConstructionZone>();
        currentConstructionZone.GapSize = new Vector2(xGap, yGap);

        return currentConstructionZone.CalculatePositionInWorld(indexOfNextBuilding);
    }

    public GameObject GetCurrentConstruction()
    {
        foreach (Transform child in buildingsParent.transform)
        {
            var farm = child.GetComponent<Farm>();
            if (!farm.IsBuilingComplete)
            {
                return child.gameObject;
            }
        }

        return null;
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
        }
    }

    private void CreateBuildingsParentObject()
    {
        Transform buildingsParentTransform = transform.Find(cBuildingsParentName);
        if (!buildingsParentTransform)
        {
            buildingsParent = new GameObject(cBuildingsParentName);
            buildingsParent.transform.SetParent(transform);
        }
    }

    private void DesignateConstructionZone()
    {
        bool closerToLeft = transform.position.x <= 0;
        int xPos = closerToLeft ? 500 : -500;
        int yPos = 0;

        Vector3 displacement = new Vector2(xPos, yPos);
        Vector2 size = new Vector2(400, 300);

        var newZone = Instantiate(constructionZone, transform.position + displacement, Quaternion.identity, transform);
        newZone.GetComponent<BoxCollider2D>().size = size;
        newZone.GetComponent<ConstructionZone>().BuildingType = farm;
        newZone.name = cConstructionZone;
    }

    internal void SignConstructionComplete()
    {
        var currentConstructionZone = transform.Find(cConstructionZone).GetComponent<ConstructionZone>();
        currentConstructionZone.BuildingFinished();
    }

    public void TakeResources(int num)
    {
        resources += num;
    }
}
