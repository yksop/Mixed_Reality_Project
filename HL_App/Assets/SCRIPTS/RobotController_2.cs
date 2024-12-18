using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotController_2 : MonoBehaviour
{
    public float moveSpeed = 0.75f; // Velocità di movimento
    public float rotationSpeed = 2f; // Velocità di rotazione
    public float stopDistance = 0.05f; // Distanza minima alla quale il robot si ferma dalla capsula

    private Animator animator; // Riferimento all'Animator del robot
    public bool isMoving = false; // Controlla se il robot si sta muovendo

    public GameObject capsule; // Riferimento alla capsula

    void Start()
    {
        // Ottieni il componente Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        // Inizializza il robot in stato fermo
        animator.SetBool("isWalking", false);
    }

    void Update()
    {
        // Controlla la distanza tra il robot e la capsula
        float distanceToCapsule = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                                                   new Vector3(capsule.transform.position.x, 0, capsule.transform.position.z));

        if (distanceToCapsule > stopDistance)
        {
            isMoving = true;
            // Attiva l'animazione di camminata
            animator.SetBool("isWalking", true);
            MoveRobot(capsule.transform.position);
        }
        else
        {
            isMoving = false;
            animator.SetBool("isWalking", false);
        }
    }

    private void MoveRobot(Vector3 targetPosition)
    {
        // Mantieni la stessa altezza (y) del robot
        targetPosition.y = transform.position.y;

        // Calcola la direzione verso la posizione target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Ruota il robot verso la posizione target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Muovi il robot verso la posizione target
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        Debug.Log("Moving to the target");
    }
}