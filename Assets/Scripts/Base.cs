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

        if (!isThereOngoingConstrucion)
        {
            var newConstructionPosition = SelectNewConstuctionPosition();

            var newFarm = Instantiate(farm, newConstructionPosition,
                Quaternion.identity, buildingsParent.transform);
            newFarm.GetComponent<Farm>().MyBase = this; 
        }

    }

    private Vector3 SelectNewConstuctionPosition()
    {
        int numOfCurrentBuildings = buildingsParent.transform.childCount;
        const int xGap = 30;
        const int yGap = 30;

        var farmWidth = farm.GetComponent<BoxCollider2D>().size.x;
        var farmHeight = farm.GetComponent<BoxCollider2D>().size.y;

        var box = transform.Find(cConstructionZone).GetComponent<BoxCollider2D>();

        int xLimit = Mathf.FloorToInt((box.size.x + xGap) / (farmWidth + xGap));
        int yLimit = Mathf.FloorToInt((box.size.y + yGap) / (farmHeight + yGap));

        int xPosInGrid = numOfCurrentBuildings % xLimit;
        int yPosInGrid = Mathf.FloorToInt(numOfCurrentBuildings / xLimit);

        Vector3 displacement = new Vector3(
            (1+xPosInGrid) * farmWidth + xPosInGrid * xGap,
            (1+yPosInGrid) * farmHeight + yPosInGrid * yGap,
            0);

        Vector3 startingPoint = new Vector3(
            box.transform.position.x - box.size.x / 2,
            box.transform.position.y - box.size.y / 2,
            0);

        return startingPoint + displacement;
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
        bool closerToBottom = transform.position.y <= 0;
        int xPos = closerToLeft ? 500 : -500;
        int yPos = 0; //closerToBottom ? 500 : -500;

        Vector3 displacement = new Vector2(xPos, yPos);
        Vector2 size = new Vector2(400, 300);

        var newZone = Instantiate(constructionZone, transform.position + displacement, Quaternion.identity, this.transform);
        newZone.GetComponent<BoxCollider2D>().size = size;
        newZone.name = cConstructionZone;
    }

    public void TakeResources(int num)
    {
        resources += num;
    }
}
