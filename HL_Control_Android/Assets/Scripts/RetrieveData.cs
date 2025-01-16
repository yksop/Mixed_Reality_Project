using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The RetrieveData class is responsible for tracking and displaying the player's speed and distance 
/// relative to a virtual robot in a mixed reality environment. It updates these values at regular intervals 
/// and provides functionality to reset the data and clear graphical elements from the scene.
/// </summary>
public class RetrieveData : MonoBehaviour
{
    public int FATTORE_DI_SCALA = 1;
    public TextMeshProUGUI textVelocita;
    public TextMeshProUGUI textDistanza;
    public Transform playerTransform;
    public Transform robotVirtual;
    public Transform graphContainerV;
    public Transform graphContainerP;


    private Vector2 previousPosition;
    public float playerSpeed; // Current speed
    public List<float> playerSpeeds = new List<float>(); // Speed history
    public List<float> playerDistances = new List<float>(); // Distance history

    // Start is called before the first frame update
    void Start()
    {
        if (playerTransform != null)
        {
            previousPosition = playerTransform.position;
        }

        StartCoroutine(UpdatePlayerSpeedRoutine(1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (robotVirtual == null || playerTransform == null) return;

        float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;
        textDistanza.text = $"Distance: {distance:F2} m";

        // Save the distance in the list
        //playerDistances.Add(distance);
    }

    public void StartCoroutineUpdatePlayerSpeedRoutine()
    {
        StartCoroutine(UpdatePlayerSpeedRoutine(1f));
    }

    private IEnumerator UpdatePlayerSpeedRoutine(float updateInterval = 1f)
    {
        while (true)
        {
            if (playerTransform != null && robotVirtual != null)
            {
                playerSpeed = (Vector2.Distance(playerTransform.position, previousPosition)) / FATTORE_DI_SCALA;
                textVelocita.text = $"Speed: {playerSpeed:F2} m/s";
                previousPosition = playerTransform.position;

                // Add the current speed to the list
                playerSpeeds.Add(playerSpeed);

                // Calculate the distance
                float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;

                playerDistances.Add(distance);
            }

            yield return new WaitForSeconds(updateInterval); // Update every second
        }
    }

    // Method to reset the data
    public void ResetGraphData(bool eraseDataLists)
    {
        if(eraseDataLists)
        {
            // Clear the lists
            playerSpeeds.Clear();
            playerDistances.Clear();
            Debug.Log("Graphs reset!");
        }
        // Find and destroy all GameObjects with the tag "GraphicalElement" that are children of graphContainerV
        foreach (Transform child in graphContainerV)
        {
            if (child.CompareTag("GraphicalElement"))
            {
                Destroy(child.gameObject); // Destroy the GameObject
            }
        }
        // Find and destroy all GameObjects with the tag "GraphicalElement" that are children of graphContainerP
        foreach (Transform child in graphContainerP)
        {
            if (child.CompareTag("GraphicalElement"))
            {
                Destroy(child.gameObject); // Destroy the GameObject
            }
        }

        // Recalculate speed and distance values based on current positions
        if (playerTransform != null && robotVirtual != null)
        {
            // Reset the player's previous position
            previousPosition = playerTransform.position;

            // Recalculate values for a short period (e.g., 5 updates)
            for (int i = 0; i < 5; i++)
            {
                // Simulate an update interval to calculate speed and distance
                float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;
                playerDistances.Add(distance);

                float speed = (Vector2.Distance(playerTransform.position, previousPosition)) / FATTORE_DI_SCALA;
                playerSpeeds.Add(speed);

                // Update the previous position
                previousPosition = playerTransform.position;
            }
        }

        // Restore real-time calculation with the coroutine
        StartCoroutine(UpdatePlayerSpeedRoutine(1f));
    }

}