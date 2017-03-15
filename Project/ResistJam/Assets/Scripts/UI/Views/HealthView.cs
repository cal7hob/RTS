using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{

    public BaseObject baseObject;

    public Image foreground;

    public void Setup(BaseObject baseObject)
    {
        this.baseObject = baseObject;
    }

    void Update()
    {
        foreground.fillAmount = (float) baseObject.health / (float) baseObject.maxHealth;

        transform.position = Camera.main.WorldToScreenPoint(baseObject.transform.position + (Vector3.up * baseObject.transform.localScale.y + Vector3.up));
    }
}
