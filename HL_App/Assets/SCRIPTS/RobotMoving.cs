using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMoving : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocità di movimento
    public float moveRange = 5f; // Raggio massimo da 0,0,0
    private Animator animator;

    private Vector3 targetPosition;
    private float walkTimer;
    private float waitTime = 5f; // Tempo di attesa tra i movimenti
    private bool isWaiting = false; // Controlla se il robot sta aspettando
    private float gravity = -9.8f; // Forza di gravità simulata
    private float verticalVelocity = 0f; // Velocità verticale (per simulare la caduta)

    public Transform player; // Riferimento al giocatore (Main Camera)
    private bool isChasingPlayer = false; // Controlla se il robot sta inseguendo il giocatore
    private float angryWaitTime = 5f; // Tempo di attesa prima di inseguire il giocatore  
    public float chaseDistance = 0.5f; // Distanza a cui il robot si ferma dal giocatore
    private bool chasing = false;
    


    void Start()
    {
        animator = GetComponent<Animator>();
        SetNewTargetPosition();
        chasing = false;
    }

    void Update()
    {
   // Se il robot è arrabbiato, insegue il giocatore dopo aver aspettato
        if (animator.GetBool("isScared"))
        {
            if (!isChasingPlayer)
            {
                angryWaitTime -= Time.deltaTime;
                if (angryWaitTime <= 0)
                {
                    animator.SetBool("isScared", false); // Cambia la variabile isHangry nell'animator
                    isChasingPlayer = true; // Inizia a inseguire il giocatore dopo 5 secondi
                }
            }

            if (isChasingPlayer)
            {
                ChasePlayer(); // Insegui il giocatore
            }
        }
        else
        {
            MoveRobot(); // Comportamento casuale
        }
    }

    void MoveRobot()
    {
        // Se il robot sta aspettando, non fare nulla
        if (isWaiting)
        {
            // Controlla se è tempo di smettere di aspettare
            walkTimer += Time.deltaTime;
            // Simula la gravità
            SimulaGravita();

            if (walkTimer >= waitTime)
            {
                isWaiting = false; // Finisce di aspettare
                SetNewTargetPosition(); // Imposta una nuova posizione target
                walkTimer = 0f; // Resetta il timer
            }
            return; // Esci dal metodo se il robot sta aspettando
        }

        // Muovi il robot verso la posizione target solo sugli assi X e Z
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z); // Mantieni la direzione solo su X e Z

        // Muovi il robot solo su X e Z
        Vector3 movement = horizontalDirection * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Ruota il robot verso la direzione di movimento
        if (horizontalDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed * 5);
        }

        // Simula la gravità
        verticalVelocity += gravity * Time.deltaTime;
        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0)); // Aggiorna la posizione sull'asse Y

        // Controlla se il robot ha raggiunto la posizione target
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPosition.x, 0, targetPosition.z)) < 0.1f)
        {
            isWaiting = true; // Inizia ad aspettare
            animator.SetBool("isWalking", false); // Ferma l'animazione di camminata
        }
        else
        {
            // Attiva il parametro isWalking nell'animator se si sta muovendo
            animator.SetBool("isWalking", true);
        }

        SimulaGravita();


    }


    void ChasePlayer()
    {
        // Ottieni la posizione del giocatore
        Vector3 playerPosition = player.position;
        Vector3 direction = (playerPosition - transform.position).normalized;

        // Muovi il robot verso il giocatore solo sugli assi X e Z
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        float distanceToPlayer = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), 
                                                  new Vector3(playerPosition.x, 0, playerPosition.z));

        // Se è distante più di chaseDistance, continua a muoversi
        if (distanceToPlayer > chaseDistance)
        {
            Vector3 movement = horizontalDirection * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);

            // Ruota il robot verso il giocatore
            if (horizontalDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed * 5);
            }

            // Attiva l'animazione di camminata
            animator.SetBool("isWalking", true);
        }
        else
        {
            // Ferma il movimento quando è abbastanza vicino al giocatore
            animator.SetBool("isWalking", false);
        }

        SimulaGravita();
    }

    void SetNewTargetPosition()
    {
        // Genera una nuova posizione casuale all'interno del raggio
        float x = Random.Range(-moveRange, moveRange);
        float y = transform.position.y; // Mantieni l'altezza attuale
        float z = Random.Range(-moveRange, moveRange);
        targetPosition = new Vector3(x, y, z);
    }

    void SimulaGravita()
    {
        verticalVelocity += gravity * Time.deltaTime;
        transform.Translate(new Vector3(0, verticalVelocity * Time.deltaTime, 0)); // Aggiorna la posizione sull'asse Y
        // Simula il contatto con il terreno
        if (transform.position.y <= 0) // Supponendo che il terreno sia a y = 0
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Mantieni il robot sul terreno
            verticalVelocity = 0; // Resetta la velocità verticale quando è sul terreno
        }
    }


    void OnTriggerEnter(Collider other)
    {
        // Se il robot entra in contatto con un oggetto con il tag "roccia"
        if (other.CompareTag("roccia"))
        {
            Debug.Log("Collisione con la roccia!"); // Aggiungi questo messaggio per vedere se viene rilevata la collisione
            animator.SetBool("isScared", true); // Cambia la variabile isScared nell'animator
            chasing = true;
            angryWaitTime = 2f; // Resetta il timer di attesa per inseguire il giocatore
        }
    }

}
