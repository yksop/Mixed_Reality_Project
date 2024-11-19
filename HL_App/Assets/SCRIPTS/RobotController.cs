using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float moveSpeed = 0.75f; // Velocità di movimento
    public float stopDistance = 2f; // Distanza minima alla quale il robot si ferma dalla camera
    public float moveRange = 3f; // Distanza minima alla quale il robot si ferma dalla camera
    private string triggerFightName = "inFight"; // Nome del trigger per il combattimento

    public float distanceFromPlayer = 1f; // distanza dalla quale si spaventa dal player 

    private Animator animator;
    private Vector3 targetPosition;
    private bool isMoving = false; // Controlla se il robot si sta muovendo
    private bool hasReachedTarget = false; // Controlla se il robot ha raggiunto la posizione target
    private float rotationTime = 3f; // Tempo in cui il robot rimane a guardare la camera
    private float rotationTimer = 0f; // Timer per la rotazione
    private bool staCombattendo = false;
    private bool staScappando = false;
    public Transform centerPoint; // Il centro da cui il robot calcolerà le posizioni casuali

    
    public enum EyePosition { normal, happy, angry, dead} // stato occhi 
    Renderer[] characterMaterials;
    
    public AudioSource footsteps;

    public LayerMask spatialAwareness; // Assicurati che il Layer della SLAM sia assegnato a questa variabile


    void Start()
    {
        // Ottieni il componente Animator
        animator = GetComponent<Animator>();        
            // Initialize characterMaterials
        characterMaterials = GetComponentsInChildren<Renderer>(); // Fetch all child Renderers, adjust as needed

        ChangeEyeOffset(EyePosition.normal);
        footsteps.Stop();

        // Inizializza il robot in stato fermo
        animator.SetBool("isWalking", false);
        staCombattendo = false;

        Camera mainCamera = Camera.main;
    }

    void Update()
    {
        /*         // Controlla se il robot è oltre il range di distanza dall'origine
        if (Vector3.Distance(transform.position, Vector3.zero) > moveRange)
        {
            // Ferma il movimento se oltre il range
            //isMoving = false;
            //new Vector3 CurrentPos = transform.position;
            //targetPosition = newVector3(transform.position.x + 2, transform.position.y, transform.position.z );
            targetPosition = SetNewTargetPositionRandom();
            animator.SetBool("isWalking", false); // Ferma l'animazione di camminata
            Debug.Log("Robot fermo: oltre il range di distanza dall'origine.");
            //return; // Esci dalla funzione Update
        } */

        if (isMoving)
        {
            MoveRobot(targetPosition); // Muovi il robot verso la sua posizione target
        }
        else if (hasReachedTarget && staCombattendo == true)
        {
            isMoving = false;
            // Se ha raggiunto il target, mantieni la rotazione verso la camera per 2 secondi
            RotateTowardsMainCamera();
            rotationTimer += Time.deltaTime;
            if (rotationTimer >= rotationTime)
            {
                hasReachedTarget = false; // Dopo 2 secondi, ferma la rotazione
                rotationTimer = 0f; // Resetta il timer
                staCombattendo = false;
                ChangeEyeOffset(EyePosition.normal);
            }
        }


        
        // Verifica la distanza solo sugli assi x e z tra il robot e la camera
        Vector3 robotPosition = transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;

        // Calcola la distanza solo sugli assi x e z (ignorando y)
        float distanceXZ = Vector3.Distance(new Vector3(robotPosition.x, 0, robotPosition.z), 
                                            new Vector3(cameraPosition.x, 0, cameraPosition.z));

        // Se la distanza è minore di 1, attiva la fuga
        if (distanceXZ < distanceFromPlayer && staScappando==false)
        {
            staScappando = true;
            StartCoroutine(PlayerHitCoroutine());
        }
    }

    public void SetMovingFalse()
    {
        isMoving = false;
    } 

    // Funzione per rilevare la collisione con oggetti
    void OnCollisionEnter(Collision collision)
    {
        // Verifica se l'oggetto con cui il robot ha colliso ha il tag "roccia"
        if (collision.gameObject.CompareTag("roccia"))
        {
            // Imposta il trigger "spavento" nell'animator
            animator.SetTrigger("spavento");
            Debug.Log("Robot si è spaventato dopo aver colpito una roccia.");

            // Avvia la coroutine per il movimento casuale
            StartCoroutine(SpaventoCoroutine());

        }
    }

    // Coroutine per gestire il movimento dopo aver colpito il player
    private IEnumerator PlayerHitCoroutine()
    {
        // Ottieni la posizione del player
        Camera player = Camera.main;
        if (player != null)
        {
            ChangeEyeOffset(EyePosition.dead);

            // Calcola la direzione opposta rispetto al player, limitata al piano XZ
            Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;

            // Ignora l'asse Y per calcolare la direzione nel piano XZ
            directionAwayFromPlayer.y = 0; 

            float angolo = Random.Range(-90,90);

            // Ruota la direzione di fuga di 45 gradi
            directionAwayFromPlayer = Quaternion.Euler(0, angolo, 0) * directionAwayFromPlayer;

            // Imposta una posizione target in direzione opposta al player, sempre nel piano XZ
            targetPosition = transform.position + directionAwayFromPlayer * stopDistance * 2;
            //targetPosition = SetNewTargetPositionRandom();
            // Inizia a muoversi verso la posizione target
            isMoving = true;

            // Imposta l'animazione di corsa
            animator.SetBool("isRunning", true);
            float incrementoVel = 1.5f;
            moveSpeed += incrementoVel; // Aumenta la velocità mentre si allontana

            // Se vuoi allontanare il robot per 1.5 secondi
            float runDuration = 1.5f;
            float elapsedTime = 0f;

            // Pausa di un secondo prima di girarsi
            yield return new WaitForSeconds(1f);

            // Il robot si è allontanato, ora fermiamo il movimento
            staScappando = false; 
            isMoving = false;
            animator.SetBool("isRunning", false);

            // Torna alla velocità originale
            moveSpeed -= incrementoVel;



            // Attiva il trigger "spavento" una volta che il robot ha guardato il player
            animator.SetTrigger("spavento");

            // Ruota il robot per guardare solo sull'asse Y verso il player
            Vector3 lookDirection = player.transform.position - transform.position;

            // Manteniamo solo la componente Y della rotazione
            lookDirection.y = 0; // Ignora le componenti X e Z per limitare la rotazione a Y
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            // Ruota gradualmente verso il player solo sull'asse Y
            float rotationSpeed = 2f;
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                yield return null; // Attende il frame successivo
            }

            ChangeEyeOffset(EyePosition.normal);

            Debug.Log("Il robot ha guardato il player ed è spaventato.");
            /* 
            targetPosition = SetNewTargetPositionRandom();
            isMoving = true; */
        }
        else
        {
            Debug.LogWarning("Player non trovato!");
            yield break; // Termina la coroutine se il player non viene trovato
        }
    }

    // Coroutine per gestire il movimento casuale dopo lo spavento
    private IEnumerator SpaventoCoroutine()
    {
        ChangeEyeOffset(EyePosition.dead);

        // Attendi un secondo
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("isRunning", true); // Assicurati che ci sia un'animazione di corsa


        // Inizia a correre casualmente per 5 secondi
        float runDuration = 3f;
        float changeDirectionInterval = 0.5f;
        float elapsedTime = 0f;
        float incrementoVel = 0.5f;

        // incremento velocita
        moveSpeed = moveSpeed + incrementoVel;

        isMoving = true; // Indica che il robot sta correndo

        while (elapsedTime < runDuration)
        {
            // Salva la posizione target precedente prima di impostarne una nuova
            //Vector3 previousTargetPosition = targetPosition;

            // Imposta una nuova posizione target
            targetPosition = SetNewTargetPositionRandom();

            // Aspetta 0.5 secondi prima di cambiare direzione
            yield return new WaitForSeconds(changeDirectionInterval);
            elapsedTime += changeDirectionInterval;
        }

        // Dopo 5 secondi, ferma il robot

        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false); // Ferma anche la camminata se attiva
        Debug.Log("Il robot ha smesso di correre.");
        isMoving = false;
        moveSpeed = moveSpeed - incrementoVel;
    }

    public void OnFightButtonPress()
    {
        // Ottieni la posizione della main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            ChangeEyeOffset(EyePosition.angry);

            // Calcola la direzione dalla posizione del robot verso la camera
            Vector3 directionToCamera = (mainCamera.transform.position - transform.position).normalized;

            // Calcola la distanza attuale tra il robot e la camera sugli assi X e Z
            Vector3 cameraPositionXZ = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
            Vector3 robotPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
            float distanceToCameraXZ = Vector3.Distance(robotPositionXZ, cameraPositionXZ);

            // Se il robot è più lontano della stopDistance, calcola la nuova posizione
            if (distanceToCameraXZ > stopDistance)
            {
                // Imposta la posizione target a una distanza di stopDistance dalla camera
                targetPosition = new Vector3(
                    mainCamera.transform.position.x - directionToCamera.x * stopDistance,
                    transform.position.y, // Mantieni la stessa altezza
                    mainCamera.transform.position.z - directionToCamera.z * stopDistance
                );

                isMoving = true;
                animator.SetBool("isWalking", true); // Attiva l'animazione di camminata
                Debug.Log("Robot in movimento verso la camera, si fermerà a distanza " + stopDistance + " unità.");
                staCombattendo = true;
            }
            else
            {
                // Il robot è già entro la distanza di stop, quindi non si muove
                Debug.Log("Robot è già entro la distanza di stopDistance. Non si muove.");
                isMoving = false;
                animator.SetBool("isWalking", false);
                staCombattendo = false;
            }
        }
        else
        {
            Debug.LogWarning("Main camera non trovata!");
        }
    }

    public void GoHomeButtonPress()
    {
        // Mantieni la posizione Y attuale e imposta X e Z a zero
        targetPosition = new Vector3(centerPoint.position.x, transform.position.y, centerPoint.position.z);//new Vector3(0f, transform.position.y, 0f);
        isMoving = true;
        animator.SetBool("isWalking", true);
    }

    // Funzione chiamata da Bottone 2: attiva il combattimento e muove il robot vicino alla camera
    public void OnChangePositionButtonPress()
    {
        
        ChangeEyeOffset(EyePosition.normal);
        targetPosition = SetNewTargetPositionRandom();
        isMoving = true;
        //MoveRobot(targetPosition);
        animator.SetBool("isWalking", true);
    }

    // Funzione chiamata da Bottone 2: attiva il combattimento e muove il robot vicino alla camera
    public void OnHappyButtonPress()
    {
        isMoving = false;
        animator.SetTrigger("isHappy"); // Attiva l'animazione di happy
        ChangeEyeOffset(EyePosition.happy);

    }

    /* // Funzione per muovere il robot verso la posizione target
    private void MoveRobot(Vector3 targetPosition)
    {
            // Riproduce il suono dei passi solo se non è già in riproduzione
        if (!footsteps.isPlaying)
        {
            footsteps.Play();
        }
        //animator.SetBool("isWalking", true);
        // Calcola la direzione verso la posizione target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Calcola la posizione verso cui si sta muovendo
        //Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        
        
        // Usa un Raycast sul Layer della SLAM Mesh per rilevare ostacoli
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, moveSpeed * Time.deltaTime + 0.75f, spatialAwareness))
        {
            // Se l'oggetto colpito ha il Layer della SLAM Mesh, ferma il movimento
            Debug.Log("Colpito oggetto SLAMMesh, fermo il movimento.");
            isMoving = false;
            targetPosition = transform.position;
            animator.SetBool("isWalking", false);
            footsteps.Stop();
            return;
        }

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
        if (Vector3.Distance(transform.position, targetPosition) <= 0.0075f)
        {
            isMoving = false; // Ferma il movimento
            animator.SetBool("isWalking", false); // Ferma l'animazione di camminata
            footsteps.Stop();
            hasReachedTarget = true; // Il robot ha raggiunto il target

            // Attiva il trigger di combattimento nell'Animator 
            // se sta combatendo fai lanimazione del combattimento
            if (staCombattendo == true)
            {
                animator.SetTrigger(triggerFightName);
                Debug.Log("Trigger combattimento attivato: " + triggerFightName);
            }

            rotationTimer = 0f; // Resetta il timer della rotazione
        }
    } */
    // Funzione per muovere il robot verso la posizione target
    private void MoveRobot(Vector3 targetPosition)
    {
        // Riproduce il suono dei passi solo se non è già in riproduzione
        if (!footsteps.isPlaying)
        {
            footsteps.Play();
        }

        // Calcola la direzione verso la posizione target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Ruota il robot verso la direzione di movimento
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed * 5);
        }


        // Dimensioni del box per il BoxCast (rettangolare nel piano x-y)
        Vector3 boxSize = new Vector3(0.5f, 0.1f, 0.5f); // Regola queste dimensioni in base alla larghezza e altezza desiderata
        
        // Usa un BoxCast sul Layer della SLAM Mesh per rilevare ostacoli
        if (Physics.BoxCast(transform.position, boxSize / 2, direction, out RaycastHit hit, Quaternion.identity, moveSpeed * Time.deltaTime + 0.5f, spatialAwareness))
        {
            // Se l'oggetto colpito ha il Layer della SLAM Mesh, ferma il movimento
            Debug.Log("Colpito oggetto SLAMMesh, fermo il movimento.");
            isMoving = false;
            targetPosition = transform.position;
            animator.SetBool("isWalking", false);
            footsteps.Stop();
            return;
        }

        // Muovi il robot nella direzione della destinazione
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);



        // Verifica se il robot ha raggiunto la posizione target
        if (Vector3.Distance(transform.position, targetPosition) <= 0.0075f)
        {
            isMoving = false; // Ferma il movimento
            animator.SetBool("isWalking", false); // Ferma l'animazione di camminata
            footsteps.Stop();
            hasReachedTarget = true; // Il robot ha raggiunto il target

            // Attiva il trigger di combattimento nell'Animator 
            if (staCombattendo == true)
            {
                animator.SetTrigger(triggerFightName);
                Debug.Log("Trigger combattimento attivato: " + triggerFightName);
            }

            rotationTimer = 0f; // Resetta il timer della rotazione
        }
    }

    private Vector3 SetNewTargetPosition(Vector3 newPos) /////////////// probabile prende byte in input ////////
    {
        targetPosition = newPos;
        return targetPosition;
    }

    private Vector3 SetNewTargetPositionRandom()
    {
        // Genera una nuova posizione casuale all'interno del raggio
        float x = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);
        targetPosition = new Vector3(centerPoint.position.x + x, transform.position.y, centerPoint.position.z + z);
        return targetPosition;
    }

    // Funzione per far girare il robot verso la main camera
    private void RotateTowardsMainCamera()
    {
        // Ottieni la posizione della main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calcola la direzione verso la camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0; // Ignora la componente Y per mantenere il robot sul piano orizzontale

            // Calcola la rotazione richiesta
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);

            // Applica la rotazione al robot in modo fluido
            float rotationSpeed = 50f;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Debug.Log("Robot ruotato verso la main camera.");
        }
        else
        {
            Debug.LogWarning("Main camera non trovata!");
        }
    }

    void ChangeEyeOffset(EyePosition pos)
    {
        Vector2 offset = Vector2.zero;

        switch (pos)
        {
            case EyePosition.normal:
                offset = new Vector2(0, 0);
                break;
            case EyePosition.happy:
                offset = new Vector2(.33f, 0);
                break;
            case EyePosition.angry:
                offset = new Vector2(.66f, 0);
                break;
            case EyePosition.dead:
                offset = new Vector2(.33f, .66f);
                break;
            default:
                break;
        }

        for (int i = 0; i < characterMaterials.Length; i++)
        {
            if (characterMaterials[i].transform.CompareTag("PlayerEyes"))
                characterMaterials[i].material.SetTextureOffset("_MainTex", offset);
        }
    }
}
