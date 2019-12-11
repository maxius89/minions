using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionZone : MonoBehaviour
{
    public GameObject BuildingType { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 GapSize { get; set; }

    private int numberOfBuildings;

    void Start()
    {
        Size = GetComponent<BoxCollider2D>().size;
        numberOfBuildings = 0;
    }

    public Vector3 CalculatePositionInWorld(int indexOfBuilding)
    {
        var displacement = CalculatePositionInZone(indexOfBuilding);
        var startingPoint = CalculateStartingPoint();

        return startingPoint + displacement;
    }

    public bool IsZoneFull()
    {
        return CalculateMaxNumberOfBuildings() <= numberOfBuildings;
    }

    public void BuildingFinished()
    {
        numberOfBuildings++;
    }

    private Vector2 GetBuildingSize()
    {
        return BuildingType.GetComponent<BoxCollider2D>().size;
    }

    private int CalculateMaxNumberOfBuildings()
    {
        CalculateGridLimits(out int xLimit, out int yLimit);
        return xLimit * yLimit;
    }

    private void CalculateGridLimits(out int xLimit, out int yLimit)
    {
        var buildingSize = GetBuildingSize();
        xLimit = Mathf.FloorToInt((Size.x + GapSize.x) / (buildingSize.x + GapSize.x));
        yLimit = Mathf.FloorToInt((Size.y + GapSize.y) / (buildingSize.y + GapSize.y));
    }

    private Vector2 GetNextCellPosition(int indexOfBuilding)
    {
        CalculateGridLimits(out int xLimit, out _);

        int xPos = indexOfBuilding % xLimit;
        int yPos = Mathf.FloorToInt(indexOfBuilding / xLimit);

        return new Vector2(xPos, yPos);
    }

    private Vector3 CalculatePositionInZone(int indexOfBuilding)
    {
        var cellIndex = GetNextCellPosition(indexOfBuilding);
        var buildingSize = GetBuildingSize();
        return new Vector3(
                (1 + cellIndex.x) * buildingSize.x + cellIndex.x * GapSize.x,
                (1 + cellIndex.y) * buildingSize.y + cellIndex.y * GapSize.y,
                0);
    }

    private Vector3 CalculateStartingPoint()
    {
        var zoneCenter = GetComponent<BoxCollider2D>();
        return new Vector3(
                zoneCenter.transform.position.x - zoneCenter.size.x / 2,
                zoneCenter.transform.position.y - zoneCenter.size.y / 2,
                0);
    }
}
