using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : BaseObject
{

    public enum ResourceType
    {
        Tree,
        Cow,
        Farm,
        GoldMine,
        StoneMine
    };

    public ResourceType resourceObjectType;

    public override void Dead()
    {
        canMove = false;
    }

    public bool needsToBeDead = false;

    public bool canMove = false;

    public bool canGather = false;

    public Resource resources;
    private Resource resourcesBeginning;
    public Resource.Resources resourceType;

    public int GetCurrentResources()
    {
        if (resourceType == Resource.Resources.Wood)
            return resources.wood;
        else if (resourceType == Resource.Resources.Food)
            return resources.food;
        else if (resourceType == Resource.Resources.Gold)
            return resources.gold;
        else if (resourceType == Resource.Resources.Stone)
            return resources.stone;
        else
        {
            return 0;
        }
    }

    public int GetBeginningResources()
    {
        if (resourceType == Resource.Resources.Wood)
            return resourcesBeginning.wood;
        else if (resourceType == Resource.Resources.Food)
            return resourcesBeginning.food;
        else if (resourceType == Resource.Resources.Gold)
            return resourcesBeginning.gold;
        else if (resourceType == Resource.Resources.Stone)
            return resourcesBeginning.stone;
        else
        {
            return 0;
        }
    }

    public void Start()
    {
        resourcesBeginning = resources;
    }

    void Update()
    {
        base.Update();

        HandleGathering();;
    }

    void HandleGathering()
    {
        if (health <= 0)
            return;

        if (unitObjectsTargetted.Count > 0)
        {
            for (int i = 0; i < unitObjectsTargetted.Count; i++)
            {
                if (needsToBeDead)
                {
                    if(health > 0)
                        GameManager.instance.Attack(unitObjectsTargetted[i], this);
                    else
                    {
                        needsToBeDead = false;
                    }
                    return;
                }

                // Collect resources
                if (unitObjectsTargetted[i].canCollect)
                {
                    
                }
            }
        }
    }
}
