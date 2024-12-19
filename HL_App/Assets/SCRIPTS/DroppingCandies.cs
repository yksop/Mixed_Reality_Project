using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using M2MqttUnity;

public class DroppingCandies : MonoBehaviour
{
    public AudioClip disappearSound;
    private AudioSource audioSource;
    public RobotController robotController;
    public GameObject candyPrefab; // Prefab dell'oggetto da droppare
    public GameObject mainCamera; // Riferimento alla Main Camera

    private GameObject candy_save; // candy to be saved

    private bool isDropping = false;
    private bool isFirstTime = true;
    public int takeDonutCounter = 0;
    public BaseClient baseClient;

    private GameObject[] candies;

    private HashSet<GameObject> processedCandies = new HashSet<GameObject>(); // Elenco delle caramelle già processate

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isFirstTime = true;
        isDropping = false;
    }

    void Update()
    {
        if (robotController.isMoving && !isDropping)
        {
            StartCoroutine(DropCandies());
        }

        // Calcola la distanza tra il candy e la main camera
        candies = GameObject.FindGameObjectsWithTag("Candy");
        //Debug.Log("Candies: " + candies.Length);
        foreach (GameObject candy in candies)
        {
            if (processedCandies.Contains(candy))
            {
                continue; // Salta le caramelle già processate
            }
            float distanceToCamera = Vector2.Distance(new Vector2(candy.transform.position.x, candy.transform.position.z),
                                                      new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z));
            //Debug.Log("Distance to camera: " + distanceToCamera); // Aggiungi questo log

            if (distanceToCamera <= 1f)
            {
                //Debug.Log("Candy is close to the camera"); 
                audioSource.PlayOneShot(disappearSound);
                //StartCoroutine(DestroyAfterSound(candy, isFirstTime));
                DestroyCandySingular(candy, isFirstTime);
                baseClient.SendCandyCount(takeDonutCounter);
            }
        }
    }

    private IEnumerator DropCandies()
    {
        isDropping = true;
        while (robotController.isMoving)
        {
            Vector3 dropPosition = new Vector3(robotController.transform.position.x, 0.5f, robotController.transform.position.z);
            GameObject candy = Instantiate(candyPrefab, dropPosition, Quaternion.identity);
            candy.tag = "Candy";
            candy.SetActive(true);
            yield return new WaitForSeconds(2f);
        }
        isDropping = false;
    }

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

    private  void DestroyCandySingular(GameObject candy, bool isFirstTime)
    {
        if (isFirstTime)
        {
            candy.SetActive(false);
            //Debug.Log("Candy inactive.");
        }
        else
        {
            Destroy(candy);
            //Debug.Log("Candy destroyed.");
        }
        takeDonutCounter++;
        Debug.Log("Donuts taken: " + takeDonutCounter);
    } 

    public void DestroyAllCandies()
    {
        candies = GameObject.FindGameObjectsWithTag("Candy");
        if (candies == null || candies.Length == 0)
        {
            Debug.LogWarning("Candies list is empty or null.");
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
        takeDonutCounter = count;
    }
<<<<<<< Updated upstream

    public void ToggleDropping()
    {
        isDropping = !isDropping;
    }
    
=======
>>>>>>> Stashed changes
} 


