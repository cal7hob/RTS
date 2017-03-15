using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SelectionManager.instance.selectedObjectsUpdate += SelectedObjectsUpdated;
    }

    void SelectedObjectsUpdated()
    {
        if (SelectionManager.instance.selectedObjects.Count <= 0)
        {
            UIManager.instance.Show(null);
            return;
        }

        BaseObject.ObjectType check = SelectionManager.instance.selectedObjects[0].objectType;
        bool sameType = SelectionManager.instance.selectedObjects.Count(o => o.objectType == check) == SelectionManager.instance.selectedObjects.Count;

        // Check the types
        if (sameType)
            UIManager.instance.Show(SelectionManager.instance.selectedObjects[0]);

        // Draw Hp
        SelectionManager.instance.selectedObjects.ForEach(x => { UIManager.instance.CreateHealthBar(x); });
    }

    public BuildingObject Build(BuildingObject buildingObject)
    {
        Debug.Log("Building: " + buildingObject.buildingType);

        if (GameManager.instance.Purchase(buildingObject.cost, false))
        {
            GameObject newBuidling = Instantiate(buildingObject.gameObject) as GameObject;
            return newBuidling.GetComponent<BuildingObject>();
        }
        return null;
    }

    public UnitObject CreateUnit(UnitObject unitObject, Vector3 position, UnitObject.Activity targetActivity, Cell targetCell)
    {
        if (GameManager.instance.maxUnits <= ObjectManager.instance.unitObjectsPlaced.Count)
            return null;

        GameObject newUnitObj = Instantiate(unitObject.gameObject, position, Quaternion.identity) as GameObject;
        UnitObject newUnit = newUnitObj.GetComponent<UnitObject>();
        newUnit.ownerUser = TeamManager.instance.curUser;
        newUnit.navAgent = newUnit.GetComponent<NavAgent>();
        newUnit.activity = targetActivity;
        newUnit.navAgent.targetCell = targetCell;
        ObjectManager.instance.unitObjectsPlaced.Add(newUnit);

        return newUnit;
    }
}
