using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private int resources = 0;
    [SerializeField] GameObject minion;
    [SerializeField] int minionCost = 30;

    private void Update()
    {
        if (resources >= minionCost)
        {
            GameObject newMinion = Instantiate(minion, transform.position, Quaternion.identity) as GameObject;
            newMinion.GetComponent<Minion>().setBase(this);          

            resources -= minionCost;
        }
    }

    public void TakeResources(int num)
    {
        resources += num;
    }
}
