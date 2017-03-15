using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float movementSpeed = 2;

    public float sensitivity = 20;

    public bool canMove = true;

    private Vector2 mousePosition
    {
        get { return Input.mousePosition; }
    }

    private Vector3 movementVelocity;

    void Update()
    {
        // Left
        if (mousePosition.x < sensitivity)
        {
            // Bottom Left corner
            if (mousePosition.y < sensitivity)
            {
                movementVelocity.x = -1;
                movementVelocity.z = 0;
            }
            // Top Left Corner
            else if (mousePosition.y > Screen.height - sensitivity)
            {
                movementVelocity.x = 0;
                movementVelocity.z = 1;
            }
            else
            {
                movementVelocity.x = -1;
                movementVelocity.z = 1;
            }
        }
        // Bottom
        else if (mousePosition.y < sensitivity)
        {
            // Bottom Right Corner
            if (mousePosition.x > Screen.width - sensitivity)
            {
                movementVelocity.x = 0;
                movementVelocity.z = -1;
            }
            else
            {
                movementVelocity.z = -1;
                movementVelocity.x = -1;
            }
        }
        // Top
        else if (mousePosition.y > Screen.height - sensitivity)
        {
            // Top Right Corner
            if (mousePosition.x > Screen.width - sensitivity)
            {
                movementVelocity.x = 1;
                movementVelocity.z = 0;
            }
            else
            {
                movementVelocity.z = 1;
                movementVelocity.x = 1;
            }
        }
        // Right
        else if (mousePosition.x > Screen.width - sensitivity)
        {
            movementVelocity.z = -1;
            movementVelocity.x = 1;
        }
        else
        {
            movementVelocity = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;
        transform.position = Vector3.Lerp(transform.position, movementVelocity + transform.position, movementSpeed * Time.deltaTime);
    }
}
