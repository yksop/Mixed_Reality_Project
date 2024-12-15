using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GettingTrajectory : MonoBehaviour
{
    public GameObject targetObject; // Public GameObject to be touched or clicked
    public RectTransform canvas; // The canvas where points will be drawn
    public GameObject pointPrefab; // Prefab for the points (e.g., a small UI Image)
    private List<GameObject> points = new List<GameObject>(); // Stores active points
    private List<Vector2> trajectory = new List<Vector2>(); // Stores the trajectory
    private bool isDragging = false; // Tracks if a drag is in progress

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleInput(touch.position, touch.phase);
        }
        // Check for mouse input
        else if (Input.GetMouseButtonDown(0)) // Mouse click starts drag
        {
            HandleInput(Input.mousePosition, TouchPhase.Began);
        }
        else if (Input.GetMouseButton(0)) // Mouse is dragging
        {
            HandleInput(Input.mousePosition, TouchPhase.Moved);
        }
        else if (Input.GetMouseButtonUp(0)) // Mouse click ends drag
        {
            HandleInput(Input.mousePosition, TouchPhase.Ended);
        }
    }

    // Handles both touch and mouse input phases
    private void HandleInput(Vector2 inputPosition, TouchPhase phase)
    {
        if (IsTouchingTarget(inputPosition))
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    // Start a new drag
                    trajectory.Clear();
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    // Update the trajectory during the movement
                    if (isDragging)
                    {
                        trajectory.Add(inputPosition);
                        CreatePoint(inputPosition); // Create a point on the canvas
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    // End the drag
                    isDragging = false;
                    StartCoroutine(KeepPointsActive(10f)); // Start a coroutine to remove points after 10 seconds
                    ProcessTrajectory();
                    break;
            }
        }
    }

    // Check if the input is on the targetObject
    private bool IsTouchingTarget(Vector2 inputPosition)
    {
        // Perform a raycast from the input point on the screen
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the raycast hits the targetObject
            return hit.collider.gameObject == targetObject;
        }
        return false;
    }

    // Create a point on the canvas at the specified screen position
    private void CreatePoint(Vector2 screenPosition)
    {
        // Instantiate the point prefab as a child of the canvas
        GameObject point = Instantiate(pointPrefab, canvas);
        point.transform.position = screenPosition; // Position the point on the canvas
        points.Add(point); // Add the point to the list of active points
    }

    // Coroutine to remove points after a delay
    private IEnumerator KeepPointsActive(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Remove all points after the delay
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
        points.Clear();
    }

    // Process the trajectory
    private void ProcessTrajectory()
    {
        Debug.Log("Trajectory recorded: " + trajectory.Count + " points.");
        foreach (Vector2 point in trajectory)
        {
            Debug.Log("Point: " + point);
        }
    }
}
