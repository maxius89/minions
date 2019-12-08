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
    [SerializeField] int minionCost = 30;
    [SerializeField] private Color teamColor;

    public Color TeamColor { get { return teamColor; } }
    private GameObject minionsParent;
    private GameObject buildingsParent;
    const string cMinionsParentName = "Minions";
    const string cBuildingsParentName = "Buildings";

    private void Start()
    {
        var spriteRenderer = transform.Find("Base Sprite").gameObject;
        spriteRenderer.GetComponent<SpriteRenderer>().color = teamColor;
        CreateMinionsParentObject();
        CreateBuildingsParentObject();
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
            var offsetFromBase = new Vector3(100, 100);
            var newFarm = Instantiate(farm, transform.position + offsetFromBase,
                Quaternion.identity, buildingsParent.transform);
            newFarm.GetComponent<Farm>().MyBase = this;
        }

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

    public void TakeResources(int num)
    {
        resources += num;
    }
}
