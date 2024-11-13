using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Aggiungi questa riga per accedere agli elementi UI

public class RobotMoveOnButtonPress : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocità di movimento
    public float moveRange = 5f; // Raggio massimo di spostamento dal centro
    private Animator animator;

    private Vector3 targetPosition;
    private bool isMoving = false; // Controlla se il robot si sta muovendo
    public Transform centerPoint; // Il centro da cui il robot calcolerà le posizioni casuali

    void Start()
    {
        animator = GetComponent<Animator>();
        // Il robot inizia fermo
        animator.SetBool("isWalking", false);
    }

    void Update()
    {
        if (isMoving)
        {
            MoveRobot(); // Muovi il robot verso la sua posizione target
        }
    }

    public void OnButtonPress()
    {
        if (!isMoving) // Solo se il robot non si sta muovendo
        {
            SetNewTargetPosition(); // Scegli una nuova posizione target casuale
            isMoving = true;
            animator.SetBool("isWalking", true); // Attiva l'animazione di camminata
        }
    }

    // Funzione per scegliere una nuova posizione casuale
    void SetNewTargetPosition()
    {
        // Genera una nuova posizione casuale all'interno del raggio
        float x = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);
        targetPosition = new Vector3(centerPoint.position.x + x, transform.position.y, centerPoint.position.z + z);
    }

    // Funzione per muovere il robot verso la posizione target
    void MoveRobot()
    {
        // Calcola la direzione verso la posizione target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Muovi il robot nella direzione della destinazione
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Ruota il robot verso la direzione di movimento
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed * 5);
        }

        // Verifica se il robot ha raggiunto la posizione target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false; // Ferma il movimento
            animator.SetBool("isWalking", false); // Ferma l'animazione di camminata
        }
    }
}
