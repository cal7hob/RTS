using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    // UI Manager
    public static UIManager instance;

    public GameObject actionGameObject;

    public Transform actionsHolder;
    public Transform upgradesHolder;

    public Text infoText;
    public Text descriptionText;

    // Hover Manager
    public Text[] textBox;
    public GameObject holderGameObject;

    // Health
    public List<HealthView> healthBars = new List<HealthView>();
    public GameObject healthBarGameObject;
    public Transform healthHolder;

    // Game Values
    public Text woodText;
    public Text foodText;
    public Text goldText;
    public Text stoneText;
    public Text unitsText;

    public Sprite destroySprite;

    public Text civilizationText;

    public void CreateHealthBar(BaseObject baseObject)
    {
        HealthView newView = GameObject.Instantiate(healthBarGameObject).GetComponent<HealthView>();
        healthBars.Add(newView);
        newView.Setup(baseObject);
        newView.transform.SetParent(healthHolder.transform);
    }

    public void SetText(string text)
    {
        if (String.IsNullOrEmpty(text))
        {
            holderGameObject.SetActive(false);
            return;
        }

        holderGameObject.SetActive(true);
        for (int i = 0; i < textBox.Length; i++)
        {
            textBox[i].text = text;
        }
    }

    public void ClearUpgrades()
    {
        for (int i = 0; i < upgradesHolder.childCount; i++)
        {
            Destroy(upgradesHolder.GetChild(i).gameObject);
        }
    }

    public void ClearActions()
    {
        for (int i = 0; i < actionsHolder.childCount; i++)
        {
            Destroy(actionsHolder.GetChild(i).gameObject);
        }
    }

    public void ClearHp()
    {
        for (int i = 0; i < healthHolder.childCount; i++)
        {
            Destroy(healthHolder.GetChild(i).gameObject);
        }
    }

    private BaseObject showBaseObject;

    public void Show(BaseObject baseObject)
    {
        if (baseObject == null)
        {
            UIManager.instance.ClearAll();
            UIManager.instance.infoText.text = "";
            showBaseObject = null;
            return;
        }

        showBaseObject = baseObject;

        UIManager.instance.ClearAll();
        UIManager.instance.infoText.text = ""; 
        if (baseObject.objectType == BaseObject.ObjectType.Building)
        {
            BuildingObject buildingObject = baseObject.GetComponent<BuildingObject>();
            // Actions
            if (buildingObject.built)
            {
                // Units to create
                if (buildingObject.creatableUnits.Length > 0)
                {
                    for (int i = 0; i < buildingObject.creatableUnits.Length; i++)
                    {
                        if (GameManager.instance.RequirementsReached(buildingObject.creatableUnits[i].unit.cost.requirements))
                        {
                            CreateAction(buildingObject.ActionPressed, GameManager.instance.GenerateDescription(buildingObject.creatableUnits[i].unit.description, buildingObject.creatableUnits[i].unit.cost), i, buildingObject.creatableUnits[i].unit.icon);
                        }
                    }
                }
            }
            CreateAction(buildingObject.ActionPressed, "Destroy - Removes building", -1, destroySprite);
        }
        else if (baseObject.objectType == BaseObject.ObjectType.Resource)
        {
            ResourceObject resourceObject = baseObject.GetComponent<ResourceObject>();
        }
        else if (baseObject.objectType == BaseObject.ObjectType.Unit)
        {
            UnitObject unitObject = baseObject.GetComponent<UnitObject>();
            // Actions
            if (unitObject.canBuild)
            {
                for (int i = 0; i < ObjectManager.instance.buildingObjects.Length; i++)
                {
                    if (GameManager.instance.RequirementsReached(ObjectManager.instance.buildingObjects[i].cost.requirements))
                    {
                        CreateAction(unitObject.ActionPressed, GameManager.instance.GenerateDescription(ObjectManager.instance.buildingObjects[i].description, ObjectManager.instance.buildingObjects[i].cost), i, ObjectManager.instance.buildingObjects[i].sprite);
                    }
                }
            }
            CreateAction(unitObject.ActionPressed, "Destroy - Removes Unit", -1, destroySprite);
        }
    }

    public void ShowUI(BaseObject baseObject)
    {
        if (baseObject == null)
            return;
        
        UIManager.instance.infoText.text = "";
        if (String.IsNullOrEmpty(baseObject.ownerUser.name) == false)
            UIManager.instance.infoText.text = "<color=#" + GameManager.instance.ColorToHex(baseObject.ownerUser.teamColor) + ">Owner - " + baseObject.ownerUser.name + "</color>\n";
        if (baseObject.objectType == BaseObject.ObjectType.Building)
        {
            BuildingObject buildingObject = baseObject.GetComponent<BuildingObject>();

            // Info
            UIManager.instance.infoText.text += "<b>" + buildingObject.buildingType + "</b> \n"
                                               + "Health: " + (int)buildingObject.health + "/" + (int)buildingObject.maxHealth
                                               + ((buildingObject.damage > 0) ? "Damage: " + buildingObject.damage : "");
        }
        else if (baseObject.objectType == BaseObject.ObjectType.Resource)
        {
            ResourceObject resourceObject = baseObject.GetComponent<ResourceObject>();
            // Info
            UIManager.instance.infoText.text += "<b>" + resourceObject.resourceType + "</b> \n"
                                               + "Health: " + resourceObject.health + "/" + resourceObject.maxHealth + "\n"
                                               + "Resources: " + resourceObject.GetCurrentResources() + "/" + resourceObject.GetBeginningResources();
        }
        else if (baseObject.objectType == BaseObject.ObjectType.Unit)
        {
            UnitObject unitObject = baseObject.GetComponent<UnitObject>();
            // Info
            UIManager.instance.infoText.text += "<b>" + unitObject.unitType + "</b> \n"
                                                + "Damage: " + unitObject.damage + "\n"
                                                + "Health: " + unitObject.health + "/" + unitObject.maxHealth + "\n"
                                                + (String.IsNullOrEmpty(unitObject.GetResourcesString()) ? "" : "Resources:" + unitObject.GetResourcesString());
        }
    }

    public void CreateAction(Action<int> inputAction, string description, int id, Sprite sprite)
    {
        GameObject newAction = Instantiate(actionGameObject);
        newAction.transform.SetParent(actionsHolder, true);
        newAction.transform.position = Vector3.zero;
        ActionView view = newAction.GetComponent<ActionView>();
        view.Setup(inputAction, description, id, sprite);
    }

    public void ClearAll()
    {
        ClearHp();
        ClearActions();
        ClearUpgrades();
    }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        holderGameObject.transform.position = Input.mousePosition;

        if (SelectionManager.instance.selectedObjects.Count <= 0)
        {
            descriptionText.text = "";
            infoText.text = "";
        }
        if (SelectionManager.instance.selectedObjects.Count > 1)
        {
            infoText.text = "Multiple objects selected";
        }

        ShowUI(showBaseObject);
    }
}
