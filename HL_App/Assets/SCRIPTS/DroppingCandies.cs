using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using M2MqttUnity;

public class DroppingCandies : MonoBehaviour
{
    public AudioClip disappearSound; // Sound to play when candy disappears
    private AudioSource audioSource; // Audio source component
    public RobotController robotController; // Reference to the robot controller
    public GameObject candyPrefab; // Prefab of the candy object to drop
    public GameObject mainCamera; // Reference to the main camera

    private GameObject candy_save; // Candy to be saved

    private bool isDropping = false; // Flag to check if candies are being dropped
    private bool isFirstTime = true; // Flag to check if it's the first time
    public int takeDonutCounter = 0; // Counter for taken donuts
    public bool canDrop = false; // Flag to allow dropping candies
    public BaseClient baseClient; // Reference to the base client

    private GameObject[] candies; // Array to store candies

    private HashSet<GameObject> processedCandies = new HashSet<GameObject>(); // List of processed candies

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the audio source component
        isFirstTime = true; // Initialize isFirstTime to true
        isDropping = false; // Initialize isDropping to false
    }

    void Update()
    {
        if (robotController.isMoving && !isDropping && canDrop)
        {
            StartCoroutine(DropCandies()); // Start dropping candies if conditions are met
        }

        // Calculate the distance between the candy and the main camera
        candies = GameObject.FindGameObjectsWithTag("Candy");
        //Debug.Log("Candies: " + candies.Length);
        foreach (GameObject candy in candies)
        {
            if (processedCandies.Contains(candy))
            {
                continue; // Skip already processed candies
            }
            float distanceToCamera = Vector2.Distance(new Vector2(candy.transform.position.x, candy.transform.position.z),
                                                      new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z));
            //Debug.Log("Distance to camera: " + distanceToCamera); // Add this log

            if (distanceToCamera <= 1f)
            {
                //Debug.Log("Candy is close to the camera"); 
                audioSource.PlayOneShot(disappearSound); // Play disappear sound
                //StartCoroutine(DestroyAfterSound(candy, isFirstTime));
                DestroyCandySingular(candy, isFirstTime); // Destroy the candy
                baseClient.SendCandyCount(takeDonutCounter); // Send the candy count
            }
        }
    }

    private IEnumerator DropCandies()
    {
        isDropping = true; // Set isDropping to true
        Vector3 lastPosition = robotController.transform.position; // Store the initial position
        float distanceTraveled = 0f; // Initialize the distance traveled

        while (robotController.isMoving)
        {
            Vector3 currentPosition = robotController.transform.position; // Get the current position
            distanceTraveled += Vector3.Distance(lastPosition, currentPosition); // Calculate the distance traveled
            lastPosition = currentPosition; // Update the last position

            if (distanceTraveled >= 1f) // Check if the robot has traveled 0.5 meters
            {
                Vector3 dropPosition = new Vector3(currentPosition.x, 0.025f, currentPosition.z);
                GameObject candy = Instantiate(candyPrefab, dropPosition, Quaternion.identity); // Instantiate a new candy
                candy.tag = "Candy"; // Set the tag to "Candy"
                candy.SetActive(true); // Activate the candy
                distanceTraveled = 0f; // Reset the distance traveled
            }

            yield return null; // Wait for the next frame
        }
        isDropping = false; // Set isDropping to false
    }
    /* private IEnumerator DropCandies()
    {
        isDropping = true; // Set isDropping to true
        while (robotController.isMoving)
        {
            Vector3 dropPosition = new Vector3(robotController.transform.position.x, 0.05f, robotController.transform.position.z);
            GameObject candy = Instantiate(candyPrefab, dropPosition, Quaternion.identity); // Instantiate a new candy
            candy.tag = "Candy"; // Set the tag to "Candy"
            candy.SetActive(true); // Activate the candy
            yield return new WaitForSeconds(2f); // Wait for 2 seconds
        }
        isDropping = false; // Set isDropping to false
    } */

    /* private IEnumerator DestroyAfterSound(GameObject candy, bool isFirstTime)
    {
        candy.tag = "Untagged";
        yield return new WaitForSeconds(disappearSound.length*0.5f);
        if(isFirstTime)
        {
            isFirstTime = false;
            candy.SetActive(false);
            //Debug.Log("Candy inactive.");
        }else{
            Destroy(candy);
            //Debug.Log("Candy destroyed.");
        }
        takeDonutCounter++;
        //Debug.Log("Donuts taken: " + takeDonutCounter);
    }  */

    public void ToggleCandyDrop()
    {
        canDrop = !canDrop; // Toggle the canDrop flag
    }

    private void DestroyCandySingular(GameObject candy, bool isFirstTime)
    {
        if (isFirstTime)
        {
            candy.SetActive(false); // Deactivate the candy
            //Debug.Log("Candy inactive.");
        }
        else
        {
            Destroy(candy); // Destroy the candy
            //Debug.Log("Candy destroyed.");
        }
        takeDonutCounter++; // Increment the donut counter
        //Debug.Log("Donuts taken: " + takeDonutCounter);
    } 

    public void DestroyAllCandies()
    {
        candies = GameObject.FindGameObjectsWithTag("Candy"); // Find all candies
        if (candies == null || candies.Length == 0)
        {
            Debug.LogWarning("Candies list is empty or null."); // Log a warning if no candies are found
            return;
        }

        // Store a reference to the first candy
        GameObject candy_save = candies[0];

        // Destroy all other candies
        for (int i = 1; i < candies.Length; i++) // Start from index 1 to skip the first
        {
            Destroy(candies[i]);
        }

        // // Clear the list but keep the first GameObject
        // candies.Clear();
        // candies.Add(candy_save);

        //Debug.Log("All candies destroyed except the first one.");
    }

    public void SetCounter(int count)
    {
        takeDonutCounter = count; // Set the donut counter
        Debug.Log("Donuts Reset -> donut: " + takeDonutCounter);
    }
}