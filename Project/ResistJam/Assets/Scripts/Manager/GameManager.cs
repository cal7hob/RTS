using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public AdvanceTypes[] advancements;
    public int currentAdvancement;

    public Resource resource;

    public int maxUnits
    {
        get
        {
            return
                ObjectManager.instance.buildingObjectsPlaced.Count(
                    x => x.built && (x.buildingType == BuildingObject.BuildingType.House || x.buildingType == BuildingObject.BuildingType.Center)) * 5;
        }
    }

    // Needs to reach 1
    // Based on per second
    
    // 1/4 of a unit per second
    public float unitProductionSpeed = 0.25f;
    // 15/10 of wood per second
    public float woodCollectionSpeed = 1.5f;
    // 13/10 of food per second
    public float foodCollectionSpeed = 1.3f;

    // modifier for speeds
    public float soldierMoveSpeedModifier = 1f;

    // build speed
    public float buildSpeed = 8f;

    // speed for decomposition
    public float decompositionSpeed = 3f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UIManager.instance.stoneText.text = resource.stone.ToString();
        UIManager.instance.goldText.text = resource.gold.ToString();
        UIManager.instance.woodText.text = resource.wood.ToString();
        UIManager.instance.foodText.text = resource.food.ToString();
        UIManager.instance.unitsText.text = ObjectManager.instance.unitObjectsPlaced.Count + "/" + maxUnits;

        UIManager.instance.civilizationText.text = advancements[currentAdvancement].advancement.ToString();
    }

    public bool RequirementReached(Requirement requirement)
    {
        // TODO: Use Linq
        for (int i = 0; i < requirement.buildings.Length; i++)
        {
            for (int x = 0; i < ObjectManager.instance.buildingObjectsPlaced.Count; x++)
            {
                if (ObjectManager.instance.buildingObjectsPlaced[x].buildingType == requirement.buildings[i])
                {
                    return true;
                }
            }    
        }
        
        return false;
    }

    public bool RequirementsReached(Requirement[] requirement)
    {
        for (int i = 0; i < requirement.Length; i++)
        {
            if (!RequirementReached(requirement[i]))
                return false;
        }
        return true;
    }

    public bool Purchase(Cost cost, bool completeTransaction = true)
    {
        if (resource.food < cost.resource.food || resource.wood < cost.resource.wood || resource.gold < cost.resource.gold || resource.stone < cost.resource.stone || !RequirementsReached(cost.requirements) || (int) cost.minAdvancement > currentAdvancement)
            return false;

        if (completeTransaction)
        {
            resource.food -= cost.resource.food;
            resource.wood -= cost.resource.wood;
            resource.gold -= cost.resource.gold;
            resource.stone -= cost.resource.stone;
        }

        return true;
    }

    public string GenerateDescription(string desc, Cost cost)
    {
        string newDesc = desc;
        if (cost.resource.wood > 0)
            newDesc += "\n  Wood: " + cost.resource.wood;
        if (cost.resource.food > 0)
            newDesc += "\n  Food: " + cost.resource.food;
        if (cost.resource.gold > 0)
            newDesc += "\n  Gold: " + cost.resource.gold;
        if (cost.resource.stone > 0)
            newDesc += "\n  Stone: " + cost.resource.stone;
        return newDesc;
    }

    public void Attack(BaseObject attacker, BaseObject target)
    {
        if (!attacker.damageCooldownComplete)
            return;

        Debug.Log(attacker.name + " is attacking: " + target.name);
        target.health -= attacker.damage;
        attacker.curDamageCooldown = Time.time + attacker.damageCooldown;
    }

    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    public string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }
}

[System.Serializable]
public class AdvanceTypes
{
    public enum Advancement
    {
        Nomad = 0,
        StoneAge = 1,
        Dark = 2,
        Enlightenment = 3,
        Modern = 4
    }

    public Advancement advancement;
    public Cost cost;
}

[System.Serializable]
public class Cost
{
    public Resource resource;
    public AdvanceTypes.Advancement minAdvancement;
    public Requirement[] requirements;
}

[System.Serializable]
public class Resource
{
    public int stone;
    public int gold;
    public int wood;
    public int food;

    public int GetTotal()
    {
        return (stone + gold + wood + food);
    }

    public enum Resources
    {
        Stone, Gold, Wood, Food
    }
}

[System.Serializable]
public class Requirement
{
    public BuildingObject.BuildingType[] buildings;
}

[System.Serializable]
public class Upgrade
{
    public Sprite icon;

    public Cost cost;

    public float unitProductionSpeedIncrease = 0f;
    public float woodCollectionSpeedIncrease = 0f;
    public float foodCollectionSpeedIncrease = 0f;

    public float soldierMoveSpeedModifierIncrease = 0f;
}