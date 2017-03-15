using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitObject : BaseObject
{

    public enum Activity
    {
        Idle,
        Mining,
        Chopping,
        Gathering,
        Hunting,
        Herder,
        Building,
        Attacking
    };
    public Activity activity;

    public enum UnitType
    {
        Villager,
        Troop
    };
    public UnitType unitType;

    public float speed;

    public int minTaskDistance;

    public bool canBuild = true;

    public Cost cost;

    public Sprite icon;

    public bool defaultObject;

    public Resource resources;

    [HideInInspector]
    public NavAgent navAgent;

    public string GetResourcesString()
    {
        string returnString = "";
        if (resources.wood > 0)
            returnString += "\nWood: " + resources.wood;
        if (resources.food > 0)
            returnString += "\nFood: " + resources.food;
        if (resources.gold > 0)
            returnString += "\nGold: " + resources.gold;
        if (resources.stone > 0)
            returnString += "\nStone: " + resources.stone;
        return returnString;
    }

    public void Start()
    {
        navAgent = GetComponent<NavAgent>();

        if (!defaultObject)
        {
            transform.position += Vector3.up * offset;
        }
        else
        {
            ownerUser = TeamManager.instance.curUser;
        }

        InvokeRepeating("FindSomethingTodo", 1, 1);
    }

    void FindSomethingTodo()
    {
        if (activity == Activity.Building)
        {
            BuildingObject nearestUnbuiltObject = ObjectManager.instance.buildingObjectsPlaced.FirstOrDefault(x => x.built == false && x.placed && (x.transform.position - transform.position).magnitude < minTaskDistance);
            if (nearestUnbuiltObject == null || targetBaseObject != null)
                return;

            DoSomething(nearestUnbuiltObject);
        }
    }

    public void DoSomething(BaseObject baseObject)
    {
        targetBaseObject = baseObject;

        // hit an object
        BuildingObject buildingObject = baseObject.GetComponent<BuildingObject>();
        ResourceObject resourceObject = baseObject.GetComponent<ResourceObject>();
        UnitObject unitObject = baseObject.GetComponent<UnitObject>();

        List<Cell> cells = GridManager.instance.FindCellsAroundRegion(new Vector2(baseObject.transform.position.x, baseObject.transform.position.z), new Vector2(baseObject.transform.localScale.x, baseObject.transform.localScale.z));
        float minDist = cells.Min(x => (transform.position - new Vector3(x.x, transform.position.y, x.y)).magnitude);
        navAgent.targetCell = cells.FirstOrDefault(x => (transform.position - new Vector3(x.x, transform.position.y, x.y)).magnitude <= minDist);

        if (buildingObject != null)
        {
            if (!buildingObject.built)
            {
                activity = Activity.Building;
            }
        }
        else if (unitObject != null)
        {

        }
        else if (resourceObject != null)
        {

        }
        return;
    }

    public override void ActionPressedMethod (int response)
    {
        Debug.Log("Pressed: " + response);
        BuildingObject newBuilding = BuildManager.instance.Build(ObjectManager.instance.buildingObjects[response]);
        if (newBuilding == null)
        {
            return;
        }
        newBuilding.placedCallback += PlaceResponse;
    }

    public BaseObject targetBaseObject;

    public void PlaceResponse(BuildingObject buildingObject)
    {
        activity = Activity.Building;

        DoSomething(buildingObject);
    }

    public override void Dead()
    {
        // TODO: Create some stuff on the ground
        Destroy(gameObject);
        ObjectManager.instance.unitObjectsPlaced.Remove(this);
    }

    void Update()
    {
        base.Update();

        navAgent.speed = speed;
        navAgent.offset = offset;

        DetectInput();

        DoAction();
    }

    // Do moving
    void DetectInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!SelectionManager.instance.selectedObjects.Contains(this) || SelectionManager.instance.building)
                return;

            RaycastHit hit = SelectionManager.instance.GetHoverGround();

            if (hit.transform == null)
                return;

            BaseObject hitObject = hit.transform.gameObject.GetComponent<BaseObject>();

            if (hitObject != null)
            {
                DoSomething(hit.transform.gameObject.GetComponent<BaseObject>());
                return;
            }

            if (!GridManager.instance.IsAvailable(new Vector2(hit.point.x, hit.point.z), new Vector2(transform.localScale.x, transform.localScale.z)))
                return;

            navAgent.targetCell = GridManager.instance.FindClosestCell(new Vector2(hit.point.x, hit.point.z));

            targetBaseObject = null;
            activity = Activity.Idle;
        }
    }

    void DoAction()
    {
        if (targetBaseObject == null)
            return;

        if (navAgent.currentCell == navAgent.targetCell)
        {
            targetBaseObject.ObjectAction(this);
        }
    }
}

