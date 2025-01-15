/// <summary>
/// This class is responsible for managing the activation and deactivation of a target GameObject.
/// It ensures the target GameObject is initially active and provides a method to toggle its active state.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Public variable for the GameObject to activate/deactivate
    public GameObject targetObject;

    // Start is called once, at the beginning
    void Start()
    {
        // Ensure that targetObject is not null
        if (targetObject == null)
        {
            Debug.LogWarning("TargetObject not assigned! Assign a GameObject in the Inspector.");
        } else {
            // Activate the GameObject at startup
            targetObject.SetActive(true);
        }
    }

    // Public method to toggle the GameObject
    public void ToggleGameObject()
    {
        if (targetObject != null)
        {
            // Change the active state of targetObject
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}