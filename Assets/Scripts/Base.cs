using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int resources = 0;
    [SerializeField] GameObject minion;
    [SerializeField] int minionCost = 30;
    [SerializeField] private Color teamColor;

    GameObject minionsParent;
    const string cMinionsParentName = "Minions";

    private void Start()
    {
        var spriteRenderer = transform.Find("Base Sprite").gameObject;
        spriteRenderer.GetComponent<SpriteRenderer>().color = teamColor;
        CreateMinionsParentObject();
    }

    private void Update()
    {
        if (resources >= minionCost)
        {
            GameObject newMinion = Instantiate(minion, transform.position, Quaternion.identity) as GameObject;
            newMinion.GetComponent<Minion>().setBase(this);
            newMinion.transform.SetParent(minionsParent.transform);

            resources -= minionCost;
        }
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

    public void TakeResources(int num)
    {
        resources += num;
    }

    public Color getTeamColor()
    {
        return teamColor;
    }
}
