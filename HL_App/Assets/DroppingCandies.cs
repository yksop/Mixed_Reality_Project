using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingCandies : MonoBehaviour
{
    public AudioClip disappearSound;
    private AudioSource audioSource;
    public RobotController robotController;
    public GameObject candyPrefab; // Prefab dell'oggetto da droppare
    public GameObject mainCamera; // Riferimento alla Main Camera

    private bool isDropping = false;
    private bool isFirstTime = true;
    public float takeDonutCounter = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isFirstTime = true;
    }

    void Update()
    {
        if (robotController.isMoving && !isDropping)
        {
            StartCoroutine(DropCandies());
        }

        // Calcola la distanza tra il candy e la main camera
        GameObject[] candies = GameObject.FindGameObjectsWithTag("Candy");
        foreach (GameObject candy in candies)
        {
            float distanceToCamera = Vector2.Distance(new Vector2(candy.transform.position.x, candy.transform.position.z),
                                                      new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z));
            if (distanceToCamera < 0.5f)
            {
                audioSource.PlayOneShot(disappearSound);
                StartCoroutine(DestroyAfterSound(candy, isFirstTime));
                takeDonutCounter++;
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
            candy.SetActive(true);
            yield return new WaitForSeconds(2f);
        }
        isDropping = false;
    }

    private IEnumerator DestroyAfterSound(GameObject candy, bool isFirstTime)
    {
        yield return new WaitForSeconds(disappearSound.length*0.5f);
        if(isFirstTime)
        {
            isFirstTime = false;
            candy.SetActive(false);
        }else{
            Destroy(candy);
        }
    }
}