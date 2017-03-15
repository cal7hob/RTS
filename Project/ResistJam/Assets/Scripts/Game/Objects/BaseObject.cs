using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{

    public enum ObjectType
    {
        Building,
        Resource,
        Unit
    };
    public ObjectType objectType;

    public bool destroyed;

    public int maxHealth;
    public int beginningHealth;
    public float _health;
    public float health
    {
        get { return _health; }
        set
        {
            if (value <= 0)
            {
                _health = 0;
                destroyed = true;
                if (SelectionManager.instance)
                {
                    if (SelectionManager.instance.selectedObjects.Contains(this))
                        SelectionManager.instance.selectedObjects.Remove(this);
                    SelectionManager.instance.selectedObjectsUpdate.Invoke();
                }
                Dead();
                return;
            }
            _health = value;
            if(onHealthChanged != null)
                onHealthChanged.Invoke();
        }
    }
    public delegate void OnHealthChanged();

    public OnHealthChanged onHealthChanged;

    public int damage;

    public bool damageCooldownComplete
    {
        get { return (Time.time >= curDamageCooldown); }
    }
    public bool canAttack;
    public float damageCooldown;
    [HideInInspector]
    public float curDamageCooldown;

    public bool canCollect;
    public string description;

    public float offset;

    public List<UnitObject> unitObjectsTargetted  = new List<UnitObject>();

    public User ownerUser;

    public void Awake()
    {
        health = beginningHealth;
        maxHealth = beginningHealth;
    }

    public void Update()
    {
        CleanTargettedObjects();
    }

    private void CleanTargettedObjects()
    {
        for (int i = 0; i < unitObjectsTargetted.Count; i++)
        {
            if (unitObjectsTargetted[i].targetBaseObject != this)
                unitObjectsTargetted.RemoveAt(i);
        }
    }

    public virtual void Dead() { }

    public void Kill()
    {
        health = 0;
    }

    public void ActionPressed(int index)
    {
        if (index == -1)
        {
            Kill();
            return;
        }
        ActionPressedMethod(index);
    }

    public void ObjectAction(UnitObject unitObject)
    {
        CleanTargettedObjects();
        if(!unitObjectsTargetted.Contains(unitObject))
            unitObjectsTargetted.Add(unitObject);
    }

    public virtual void ActionPressedMethod(int index) { }
}
