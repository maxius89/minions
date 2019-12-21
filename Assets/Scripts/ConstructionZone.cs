using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConstructionZone : MonoBehaviour
{
    public GameObject BuildingType { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 GapSize { get; set; }
    public int ZoneIndex { get; set; }

    private int numberOfBuildings;
    private List<Vector3> emptyGridCells;

    void Start()
    {
        numberOfBuildings = 0;
        Size = GetComponent<BoxCollider2D>().size;

        CalculateGridCells();
        SortCellsByDistanceToParent();
    }

    private void CalculateGridCells()
    {
        emptyGridCells = new List<Vector3>();
        var gridOrigin = CalculateStartingPoint();
        var buildingSize = GetBuildingSize();
        CalculateGridLimits(out int xLimit, out int yLimit);

        for (int xIndex = 0; xIndex < xLimit; xIndex++)
        {
            for (int yIndex = 0; yIndex < yLimit; yIndex++)
            {
                var xPos = (1 + xIndex) * buildingSize.x + xIndex * GapSize.x;
                var yPos = (1 + yIndex) * buildingSize.y + yIndex * GapSize.y;

                emptyGridCells.Add(new Vector3(xPos, yPos, 0) + gridOrigin);
            }
        }
    }

    private void SortCellsByDistanceToParent()
    {
        if (!transform.parent) { return; }
        var baseCoordinates = transform.parent.position;
        emptyGridCells = emptyGridCells.OrderBy(x => Vector3.Distance(baseCoordinates, x)).ToList();
    }

    public Vector3 CalculatePositionInWorld()
    {
        var position = emptyGridCells.First();
        emptyGridCells.Remove(emptyGridCells.First());
        return position;
    }

    public bool IsZoneFull()
    {
        return CalculateMaxNumberOfBuildings() <= numberOfBuildings;
    }

    private int CalculateMaxNumberOfBuildings()
    {
        CalculateGridLimits(out int xLimit, out int yLimit);
        return xLimit * yLimit;
    }

    public void BuildingFinished()
    {
        numberOfBuildings++;
    }

    private Vector2 GetBuildingSize()
    {
        return BuildingType.GetComponent<BoxCollider2D>().size;
    }

    private void CalculateGridLimits(out int xLimit, out int yLimit)
    {
        var buildingSize = GetBuildingSize();
        xLimit = Mathf.FloorToInt((Size.x + GapSize.x) / (buildingSize.x + GapSize.x));
        yLimit = Mathf.FloorToInt((Size.y + GapSize.y) / (buildingSize.y + GapSize.y));
    }

    private Vector3 CalculateStartingPoint()
    {
        var zoneCenter = GetComponent<BoxCollider2D>();
        return new Vector3(
                zoneCenter.transform.position.x - zoneCenter.size.x / 2,
                zoneCenter.transform.position.y - zoneCenter.size.y / 2,
                0);
    }

    internal void HandleConstruction(Base newBase)
    {
        if (!IsThereOngoingConstruction() && !IsZoneFull())
        {
            var newConstructionPosition = CalculatePositionInWorld();

            var newFarm = Instantiate(BuildingType, newConstructionPosition,
                Quaternion.identity, transform);
            newFarm.GetComponent<Farm>().MyBase = newBase;
        }
    }

    public bool IsThereOngoingConstruction()
    {
        bool isThereOngoingConstrucion = false;

        var farms = GetComponentsInChildren<Farm>();
        foreach (var farm in farms)
        {
            if (!farm.IsBuilingComplete)
            {
                isThereOngoingConstrucion = true;
                break;
            }
        }

        return isThereOngoingConstrucion;
    }

    public GameObject GetCurrentConstruction()
    {
        var farms = GetComponentsInChildren<Farm>();
        foreach (var farm in farms)
        {
            if (!farm.IsBuilingComplete)
            {
                return farm.gameObject;
            }
        }

        return null;
    }
}
