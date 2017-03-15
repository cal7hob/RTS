using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionView : MonoBehaviour
{

    private string description;
    private int id;
    private Sprite sprite;

    public Image image;
    public Button button;
    public EventTrigger eventTrigger;

    public void Setup(Action<int> clickCallbackAction, string description, int id, Sprite sprite)
    {
        this.description = description;
        this.id = id;
        this.sprite = sprite;

        image.sprite = this.sprite;

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => { Hover(); });
        eventTrigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => { ExitHover(); });
        eventTrigger.triggers.Add(entryExit);

        button.onClick.AddListener(() => { clickCallbackAction(id); });
    }

    void Hover()
    {
        UIManager.instance.descriptionText.text = description;
    }

    void ExitHover()
    {
        UIManager.instance.descriptionText.text = "";
    }
}
