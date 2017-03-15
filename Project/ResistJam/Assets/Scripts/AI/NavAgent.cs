using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavAgent : MonoBehaviour
{

    public float speed = 5;
    public float rotationSpeed = 8;

    private Cell _targetCell;
    public Cell targetCell
    {
        get { return _targetCell; }
        set
        {
            _targetCell = value;
            GeneratePath(_targetCell);
        }
    }

    public Cell currentCell
    {
        get { return GridManager.instance.FindClosestCell(new Vector2(transform.position.x, transform.position.z), true); }
    }

    public List<Cell> pathGenerated = new List<Cell>();
    public float distanceNeeded = 3f;

    public float offset;

    public bool moveToTarget = true;

    void Start()
    {
        targetCell = currentCell;
    }

    void Update()
    {
        MoveToTarget();
    }

    void MoveToTarget()
    {
        if (!moveToTarget)
            return;

        if (pathGenerated.Count <= 0 || pathGenerated[0] == null || !pathGenerated[0].isAvailable)
            return;

        if (GridManager.GetDistance(currentCell, pathGenerated[0]) <= distanceNeeded)
        {
            pathGenerated.RemoveAt(0);
            return;
        }

        Vector3 lastRotation = transform.localEulerAngles;
        transform.LookAt(new Vector3(pathGenerated[0].x, pathGenerated[0].yPos + offset, pathGenerated[0].y));
        Vector3 newRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(pathGenerated[0].x, pathGenerated[0].yPos + offset, pathGenerated[0].y), speed * Time.deltaTime);
    }

    public void GeneratePath(Cell targetCel)
    {
        pathGenerated.Clear();

        List<Cell> openList = GridManager.instance.FindCellAdjacent(currentCell);
        GridManager.SetParent(openList, currentCell);
        List<Cell> closedList = new List<Cell>();
        Cell finalCell = new Cell(0, 0, 0);

        // Continue till the open list is not empty
        while (openList.Count > 0)
        {
            // get costs
            openList = GridManager.SetCosts(openList, targetCell, currentCell);
            
            // find lowest costing cell
            Cell cheapestCell = GridManager.FindLowestCostCell(openList);

            if (cheapestCell.x == targetCell.x && cheapestCell.y == targetCell.y)
            {
                pathGenerated = FollowTheParent(cheapestCell, currentCell);
                pathGenerated.Add(targetCell);
                return;
            }
            else
            {
                // move cheapest node to the closed list
                openList.Remove(cheapestCell);
                closedList.Add(cheapestCell);

                // Examine each node around the cheapest node
                List<Cell> cheapestNeighbors = GridManager.instance.FindCellAdjacent(cheapestCell);

                for (int i = 0; i < cheapestNeighbors.Count; i++)
                {
                    if (!openList.Contains(cheapestNeighbors[i]) && !closedList.Contains(cheapestNeighbors[i]) && cheapestNeighbors[i].isAvailable)
                    {
                        cheapestNeighbors[i].parentCell = cheapestCell;
                        openList.Add(cheapestNeighbors[i]);    
                    }
                }
            }
        }
    }

    private List<Cell> FollowTheParent(Cell cell, Cell beginningCell)
    {
        List<Cell> returnList = new List<Cell>();
        while (cell != beginningCell)
        {
            cell = cell.parentCell;
            returnList.Add(cell);
        }
        returnList.Reverse();
        return returnList;
    }

    void OnDrawGizmosSelected()
    {
        for (int i = 0; i < pathGenerated.Count; i++)
        {
            if(i == 0)
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.red;

            Gizmos.DrawCube(new Vector3(pathGenerated[i].x, 5, pathGenerated[i].y), Vector3.one);
        }
    }
}
