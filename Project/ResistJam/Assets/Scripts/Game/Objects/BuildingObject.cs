using System.Collections;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using UnityEngine;

public class BuildingObject : BaseObject
{
    public enum BuildingType
    {
        Center,
        House,
        Mill,
        MiningCamp,
        LumberCamp,
        Tower,
        Outpost,
        Barracks,
        Archery,
        Stable
    };

    [System.Serializable]
    public struct Progress
    {
        public int healthPoint;
        public GameObject buildGameObject;
    }

    [System.Serializable]
    public struct CreatableUnit
    {
        public string name;
        public UnitObject unit;
    }

    public BuildingType buildingType;

    public Cost cost;

    public Sprite sprite;

    public bool built;
    public bool placed;

    public Progress[] buildProgresses;

    public Progress[] destroyProgress;

    public Resource.Resources[] acceptableResourceses;

    public CreatableUnit[] creatableUnits;
    public bool canUpgradeAdvancement;

    private  List<Cell> cellsUsed = new List<Cell>();

    public bool defaultObject;

    public override void ActionPressedMethod(int index)
    {
        if (index < 50)
        {
            CreateUnit(creatableUnits[index].unit);
        }
    }

    public delegate void PlacedCallback(BuildingObject buildingObject);
    public PlacedCallback placedCallback;

    public override void Dead()
    {
        // TODO: Create some stuff on the ground
        Destroy(gameObject);
        ObjectManager.instance.buildingObjectsPlaced.Remove(this);

        for (int i = 0; i < cellsUsed.Count; i++)
        {
            // enable that grid cell
            GridManager.instance.grid[new Vector2(cellsUsed[i].x, cellsUsed[i].y)].isAvailable = true;
        }
    }

    public void Place()
    {
        if (!defaultObject)
        {
            // Check again if we can purchase and also compelete the transaction
            if (!GameManager.instance.Purchase(cost))
            {
                return;
            }
            health = 1;
            built = false;
            placed = true;
        }

        ObjectManager.instance.buildingObjectsPlaced.Add(this);

        SelectionManager.instance.building = false;

        SelectionManager.instance.selectedObjects.Clear();
        if(defaultObject)
            SelectionManager.instance.selectedObjects.Add(this);
        if(!defaultObject)
            SelectionManager.instance.selectedObjectsUpdate.Invoke();

        cellsUsed = GridManager.instance.FindCellsInRegion(new Vector2(transform.position.x, transform.position.z), new Vector2(transform.localScale.x, transform.localScale.z));
        for (int i = 0; i < cellsUsed.Count; i++)
        {
            // disable that grid cell
            GridManager.instance.grid[new Vector2(cellsUsed[i].x, cellsUsed[i].y)].isAvailable = false;
        }

        ownerUser = TeamManager.instance.curUser;

        if(placedCallback != null)
            placedCallback.Invoke(this);
    }

    public void Cancel()
    {
        SelectionManager.instance.building = false;
        Destroy(gameObject);
    }
    
    void Awake()
    {
        base.Awake();
        onHealthChanged += HandleProgress;
    }

    void Start()
    {
        if(!placed)
            SelectionManager.instance.building = true;

        if (defaultObject)
            Place();
    }

    void CreateUnit(UnitObject unitObject)
    {
        Debug.Log("Creating unit: " + unitObject.unitType);
        if (GameManager.instance.Purchase(unitObject.cost))
        {
            List<Cell> randomCellList = GridManager.instance.FindCellsAroundRegion(new Vector2(transform.position.x, transform.position.z), new Vector2(transform.localScale.x, transform.localScale.z));
            Cell randomCell = randomCellList[Random.Range(0, randomCellList.Count)];
            BuildManager.instance.CreateUnit(unitObject, new Vector3(randomCell.x, randomCell.yPos, randomCell.y), UnitObject.Activity.Idle, randomCell);
        }
    }

    void Update()
    {
        base.Update();
        HandleTargets();
        if (!placed)
        {
            Vector3 point = SelectionManager.instance.GetHoverGround().point;

            if (!GridManager.instance.IsAvailable(new Vector2(point.x, point.z), new Vector2(transform.localScale.x, transform.localScale.z)))
                return;

            Cell cellToPlace = GridManager.instance.FindClosestCell(new Vector2(point.x, point.z));
            transform.position = new Vector3(cellToPlace.x, cellToPlace.yPos + offset, cellToPlace.y);

            if (Input.GetMouseButtonDown(0))
            {
                Place();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Cancel();
            }
        }
    }

    void HandleTargets()
    {
        if (health >= maxHealth)
            return;

        for (int i = 0; i < unitObjectsTargetted.Count; i++)
        {
            if (unitObjectsTargetted[i].ownerUser != ownerUser)
            {
                GameManager.instance.Attack(unitObjectsTargetted[i], this);
            }
            else
                health += GameManager.instance.buildSpeed * Time.deltaTime;
        }
    }

    void HandleProgress()
    {
        if (health <= 0)
        {
            ObjectManager.instance.buildingObjectsPlaced.Remove(this);
        }
        if (!built)
        {
            int maxHealthFound = 0;
            for (int i = 0; i < buildProgresses.Length; i++)
            {
                if (buildProgresses[i].healthPoint >= maxHealthFound)
                    maxHealthFound = buildProgresses[i].healthPoint;
            }
            if (maxHealthFound == 0)
                return;

            Progress foundProgress = new Progress();

            for (int i = 0; i < buildProgresses.Length; i++)
            {
                if (buildProgresses[i].healthPoint == maxHealthFound)
                {
                    foundProgress = buildProgresses[i];
                    foundProgress.buildGameObject.SetActive(true);
                }
                else
                {
                    foundProgress.buildGameObject.SetActive(false);
                }
            }
        }
        else
        {
            int maxHealthFound = 0;
            for (int i = 0; i < destroyProgress.Length; i++)
            {
                if (destroyProgress[i].healthPoint >= maxHealthFound)
                    maxHealthFound = destroyProgress[i].healthPoint;
            }
            if (maxHealthFound == 0)
                return;

            Progress foundProgress = new Progress();

            for (int i = 0; i < destroyProgress.Length; i++)
            {
                if (destroyProgress[i].healthPoint == maxHealthFound)
                {
                    foundProgress = destroyProgress[i];
                    foundProgress.buildGameObject.SetActive(true);
                }
                else
                {
                    foundProgress.buildGameObject.SetActive(false);
                }
            }
        }
    }
}

