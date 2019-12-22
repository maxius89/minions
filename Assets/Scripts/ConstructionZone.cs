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
    private GameObject[,] buildingGrid;

    private Vector2 BuildingSize => BuildingType.GetComponent<BoxCollider2D>().size;
    private Vector3 ZoneOrigin
    {
        get
        {
            var zoneCenter = GetComponent<BoxCollider2D>();
            return new Vector3(
                    zoneCenter.transform.position.x - zoneCenter.size.x / 2,
                    zoneCenter.transform.position.y - zoneCenter.size.y / 2,
                    0);
        }
    }

    void Start()
    {
        Size = GetComponent<BoxCollider2D>().size;
        InitGrid();
    }

    private void InitGrid()
    {
        int numX = Mathf.FloorToInt((Size.x + GapSize.x) / (BuildingSize.x + GapSize.x));
        int numY = Mathf.FloorToInt((Size.y + GapSize.y) / (BuildingSize.y + GapSize.y));

        buildingGrid = new GameObject[numX, numY];
    }

    private (Vector2 gridPos, Vector3 worldPos, bool isCellFound) FindClosestGridCell()
    {
        bool isCellFound = false;
        var xLimit = buildingGrid.GetLength(0);
        var yLimit = buildingGrid.GetLength(1);
        var baseCoordinates = transform.parent.position;

        float minDistance = float.MaxValue;
        Vector2 gridPos = Vector2.zero;
        Vector3 worldPos = Vector3.zero;
        for (int xIndex = 0; xIndex < xLimit; xIndex++)
        {
            for (int yIndex = 0; yIndex < yLimit; yIndex++)
            {
                if (buildingGrid[xIndex, yIndex] != null) { continue; }

                var currentPos = GridPosToWordPos(xIndex, yIndex);
                var distance = Vector3.Distance(baseCoordinates, currentPos);
                if (minDistance > distance)
                {
                    isCellFound = true;
                    minDistance = distance;
                    gridPos = new Vector2(xIndex, yIndex);
                    worldPos = currentPos;
                }
            }
        }
        
        return (gridPos, worldPos, isCellFound);
    }

    private Vector3 GridPosToWordPos(int xIndex, int yIndex)
    {
        var xPos = (1 + xIndex) * BuildingSize.x + xIndex * GapSize.x;
        var yPos = (1 + yIndex) * BuildingSize.y + yIndex * GapSize.y;

        Vector3 currentPos = new Vector3(xPos, yPos, 0) + ZoneOrigin;
        return currentPos;
    }

    private bool IsZoneFull()
    {
        foreach (var cell in buildingGrid)
        {
            if (cell == null) return false;
        }

        return true;
    }

    internal void HandleConstruction()
    {
        if (IsThereOngoingConstruction() || IsZoneFull()) { return; }

        (Vector2 gridPos, Vector3 worldPos, bool isCellFound) = FindClosestGridCell();
        if (!isCellFound) { return; }

        var newFarm = Instantiate(BuildingType, worldPos,
            Quaternion.identity, transform);

        buildingGrid[(int)gridPos.x, (int)gridPos.y] = newFarm;
    }

    public bool IsThereOngoingConstruction()
    {
        var farms = GetComponentsInChildren<Farm>();
        foreach (var farm in farms)
        {
            if (!farm.IsBuilingComplete) { return true; }
        }

        return false;
    }

    public GameObject GetCurrentConstruction()
    {
        var farms = GetComponentsInChildren<Farm>();
        foreach (var farm in farms)
        {
            if (!farm.IsBuilingComplete) { return farm.gameObject; }
        }

        return null;
    }
}