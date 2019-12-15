using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject resource;
    [SerializeField] float resourceSpawnTimerInSeconds = 3.0f;

    GameObject resourceParent;
    const string cResourceParentName = "Resources";

    void Start()
    {
        CreateResourceParent();
        StartCoroutine(SpawnResource());
    }

    private void CreateResourceParent()
    {
        resourceParent = GameObject.Find(cResourceParentName);
        if (!resourceParent)
        {
            resourceParent = new GameObject(cResourceParentName);
        }
    }

    IEnumerator SpawnResource()
    {
        while (true)
        {
            int xLim = Screen.currentResolution.width / 2;
            int yLim = Screen.currentResolution.height / 2;
            Vector3 targetPosition = new Vector3(Random.Range(-xLim, xLim), Random.Range(-yLim, yLim), 0);

            Instantiate(resource, targetPosition, Quaternion.identity, resourceParent.transform);
            yield return new WaitForSeconds(resourceSpawnTimerInSeconds);
        }
    }
}
