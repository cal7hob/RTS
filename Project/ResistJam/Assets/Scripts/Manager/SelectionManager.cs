using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour
{

    public static SelectionManager instance;

    public bool selecting;

    public RectTransform selectPanel;

    public Camera camera;

    public LayerMask hoverLayerMask;

    public List<BaseObject> selectedObjects = new List<BaseObject>();

    public delegate void SelectedObjectsUpdate();
    public SelectedObjectsUpdate selectedObjectsUpdate;

    public bool building;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateSelectedObject();
    }

    void UpdateSelectedObject()
    {
        if (building)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            BaseObject objects = GetHover();
            if (!Input.GetKey(KeyCode.LeftShift))
                selectedObjects.Clear();
            else
            {
                if (objects == null)
                    return;

                if (selectedObjects.Contains(objects))
                {
                    selectedObjects.Remove(objects);
                    selectedObjectsUpdate.Invoke();
                    return;
                }
            }

            if (objects != null && !selectedObjects.Contains(objects))
            {
                selectedObjects.Add(objects);
            }

            if(selectedObjectsUpdate != null)
                selectedObjectsUpdate.Invoke();
        }
    }

    public Vector2 beginMousePosition;

    void GetSelect()
    {
        if (selecting)
        {
            selectPanel.gameObject.SetActive(true);
            selectPanel.position = beginMousePosition;
            selectPanel.sizeDelta = beginMousePosition - (Vector2)Input.mousePosition;
        }
        else
        {
            selectPanel.gameObject.SetActive(false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Begin
            selecting = true;
            beginMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // End
            selecting = false;
        }

        if (Input.GetMouseButtonDown(1) && selecting)
        {
            // Cancel
            selecting = false;
        }
    }

    public BaseObject GetHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, hoverLayerMask))
        {
            return hit.collider.gameObject.GetComponent<BaseObject>();
        }
        return null;
    }

    public RaycastHit GetHoverGround()
    {
        RaycastHit hit;
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 1000, hoverLayerMask);
        return hit;
    }
}
