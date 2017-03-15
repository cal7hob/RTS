using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{

    public static ObjectManager instance;

    public BuildingObject[] buildingObjects;
    public ResourceObject[] resourceObjects;
    public UnitObject[] unitObjects;

    public List<BuildingObject> buildingObjectsPlaced;
    public List<ResourceObject> resourceObjectsPlaced;
    public List<UnitObject> unitObjectsPlaced;

    void Awake()
    {
        instance = this;
    }
}
