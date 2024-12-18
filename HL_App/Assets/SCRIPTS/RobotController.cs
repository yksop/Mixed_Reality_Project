using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float moveSpeed = 0.75f; // Velocità di movimento
    public float stopDistance = 2f; // Distanza minima alla quale il robot si ferma dalla camera
    public float moveRange = 3f; // Distanza minima alla quale il robot si ferma dalla camera
    private string triggerFightName = "inFight"; // Nome del trigger per il combattimento

    public float distanceFromPlayer = 1f; // distanza dalla quale si spaventa dal player 

    private Animator animator;
    public bool isMoving = true; // Controlla se il robot si sta muovendo
    private bool hasReachedTarget = false; // Controlla se il robot ha raggiunto la posizione target
    private float rotationTime = 3f; // Tempo in cui il robot rimane a guardare la camera
    private float rotationTimer = 0f; // Timer per la rotazione
    private bool staCombattendo = false;

    private bool staScappando = false;
    public Transform centerPoint; // Il centro da cui il robot calcolerà le posizioni casuali

    // PID CONTROLLER
	[ SerializeField ] private PIDController z_controller; // controller of foward direction of character
	[ SerializeField ] private PIDController yaw_controller; // controller of yaw rotation (forward direction) of character
	[SerializeField] private Vector3 target_position; // target destination in play-space

	[ SerializeField ] private bool is_PID_control;
	private bool facing_target;
	private bool at_target;

    private Vector2[] trajectory;
    private int traj_count = 0;

    public enum EyePosition { normal, happy, angry, dead} // stato occhi 
    Renderer[] characterMaterials;
    
    public AudioSource footsteps;

    public LayerMask spatialAwareness; // Assicurati che il Layer della SLAM sia assegnato a questa variabile

    public GameObject capsule;
    

    void Start()
    {
        // Ottieni il componente Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        // Initialize characterMaterials
        characterMaterials = GetComponentsInChildren<Renderer>(); // Fetch all child Renderers, adjust as needed

        ChangeEyeOffset(EyePosition.normal);
        footsteps.Stop();

        // Inizializza il robot in stato fermo
        animator.SetBool("isWalking", false);
        staCombattendo = false;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        if (capsule == null)
        {
            Debug.LogError("Capsule GameObject not assigned!");
            return;
        }

        target_position = capsule.transform.position; // Imposta la posizione target alla posizione della capsula
    }

    void Update()
    {

        /* UNCOMMENT TO TEST PID Controller */

        // isMoving = is_PID_control? true : false;

        /* UNCOMMENT TO TEST PID Controller */
        

        /*if ( Vector3.Distance(transform.position, target_position) <= 0.1f &&
             ++traj_count < trajectory.Length )
        {
            SetNewtarget_position(trajectory[traj_count]);
        } */

        // Imposta target_position alla posizione della capsula
        target_position = capsule.transform.position;       


        if (isMoving)
        {
            
            MoveRobot(target_position); // Muovi il robot verso la sua posizione target
        }
        /* else if (hasReachedTarget && staCombattendo == true)
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
        } */


        
        // Verifica la distanza solo sugli assi x e z tra il robot e la camera
        /* Vector3 robotPosition = transform.position;
        Vector3 cameraPosition = Camera.main.transform.position;

        // Calcola la distanza solo sugli assi x e z (ignorando y)
        float distanceXZ = Vector3.Distance(new Vector3(robotPosition.x, 0, robotPosition.z), 
                                            new Vector3(cameraPosition.x, 0, cameraPosition.z));

        // Se la distanza è minore di 1, attiva la fuga
         if (distanceXZ < distanceFromPlayer && staScappando==false)
        {
            staScappando = true;
            StartCoroutine(PlayerHitCoroutine());
        } */
    }

    public void SetMovingFalse()
    {
        isMoving = false;
    } 

    // Funzione per rilevare la collisione con oggetti
    /* void OnCollisionEnter(Collision collision)
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
    } */

    // Coroutine per gestire il movimento dopo aver colpito il player
    /* private IEnumerator PlayerHitCoroutine()
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
            target_position = transform.position + directionAwayFromPlayer * stopDistance * 2;
            //target_position = SetNewtarget_positionRandom();
            // Inizia a muoversi verso la posizione target
            isMoving = true;

            // Imposta l'animazione di corsa
            animator.SetBool("isRunning", true);
            float incrementoVel = 1.5f;
            moveSpeed += incrementoVel; // Aumenta la velocità mentre si allontana

            // Se vuoi allontanare il robot per 1.5 secondi
            //float runDuration = 1.5f;
            //float elapsedTime = 0f;

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
            
            target_position = SetNewtarget_positionRandom();
            isMoving = true; 
        }
        else
        {
            Debug.LogWarning("Player non trovato!");
            yield break; // Termina la coroutine se il player non viene trovato
        }
    } */

    // Coroutine per gestire il movimento casuale dopo lo spavento
    /* private IEnumerator SpaventoCoroutine()
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
            //Vector3 previoustarget_position = target_position;

            // Imposta una nuova posizione target
            target_position = SetNewtarget_positionRandom();

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
    } */
    /*public void OnFightButtonPress()
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
                target_position = new Vector3(
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
    } */

    /* public void GoHomeButtonPress()
    {
        // Mantieni la posizione Y attuale e imposta X e Z a zero
        target_position = new Vector3(centerPoint.position.x, transform.position.y, centerPoint.position.z);//new Vector3(0f, transform.position.y, 0f);
        isMoving = true;
        animator.SetBool("isWalking", true);
    } */

    // Funzione chiamata da Bottone 2: attiva il combattimento e muove il robot vicino alla camera
    /* public void OnChangePositionButtonPress()
    {
        
        ChangeEyeOffset(EyePosition.normal);
        target_position = SetNewtarget_positionRandom();
        isMoving = true;
        //MoveRobot(target_position);
        animator.SetBool("isWalking", true);
    } */

    // Funzione chiamata da Bottone 2: attiva il combattimento e muove il robot vicino alla camera
    /* public void OnHappyButtonPress()
    {
        isMoving = false;
        animator.SetTrigger("isHappy"); // Attiva l'animazione di happy
        ChangeEyeOffset(EyePosition.happy);

    } */

    
    // Funzione per muovere il robot verso la posizione target
    private void MoveRobot(Vector3 target_position)
    {
        // Riproduce il suono dei passi solo se non è già in riproduzione
        if (!footsteps.isPlaying)
        {
            footsteps.Play();
        }
        // Dimensioni del box per il BoxCast (rettangolare nel piano x-y)
        Vector3 boxSize = new Vector3(0.2f, 0.5f, 0.2f); // Regola queste dimensioni in base alla larghezza e altezza desiderata
        
        // // Calcola la direzione verso la posizione target
        Vector3 direction = (target_position - transform.position).normalized;

        // Usa un BoxCast sul Layer della SLAM Mesh per rilevare ostacoli
        if (Physics.BoxCast(transform.position, boxSize / 2, direction, out RaycastHit hit, Quaternion.identity, moveSpeed * Time.deltaTime + 0.5f, spatialAwareness))
        {
            // Se l'oggetto colpito ha il Layer della SLAM Mesh, ferma il movimento
            Debug.Log("Colpito oggetto SLAMMesh, fermo il movimento.");
            isMoving = false;
            //target_position = transform.position;
            animator.SetBool("isWalking", false);
            footsteps.Stop();
            return;
        }


		// Implementing PID controller to move and rotate player
		// First transform target position into characters local coordinates
		target_position.y = transform.position.y; // we do not implement any control in the vertical direction (i.e. only gravity)

		Vector3 target_direction = ( target_position - transform.position ).normalized;

		var current_angle = Vector3.SignedAngle ( Vector3.forward, transform.forward, Vector3.up );  // character argan angle
		var target_angle = Vector3.SignedAngle ( Vector3.forward, target_direction, Vector3.up ); // target argan angle
		float input_yaw = yaw_controller.UpdateAngle( Time.fixedDeltaTime, current_angle, target_angle );  // yaw_controller update

		facing_target = Mathf.Abs( input_yaw ) < yaw_controller.threshold ? true : false;

		float distance = ( target_position - transform.position ).magnitude;
		float character_vel = moveSpeed;
		//float anim_blend = 0.5f;

		float input_z = z_controller.UpdatePosition( Time.fixedDeltaTime, transform.position.z, transform.position.z + distance );

		at_target =  Mathf.Abs( input_z ) < z_controller.threshold ? true : false;

		//if ( !at_target ) // walk/run to target after facing the direction of the target
		//{
            if ( !facing_target ) // rotate until the character is facing the direction of the target
            {
                transform.Rotate (Vector3.up, input_yaw*2);
                Debug.Log("Rotating to face the target");
            }
			var desired_move_direction = new Vector3(0, 0, input_z);
			desired_move_direction = transform.TransformVector(desired_move_direction);

			transform.Translate ( desired_move_direction * Time.deltaTime * character_vel, Space.World ); // robot walks to set target
			// anim.SetFloat("Blend", anim_blend, StartAnimTime, Time.deltaTime);
            animator.SetBool("isWalking", true); // Animation as robot walks to set target

            Debug.Log("Moving to the target");
		//}

        // Verifica se il robot ha raggiunto la posizione target
        /* else //(Vector3.Distance(transform.position, target_position) <= 0.0075f)
        {
            isMoving = false; // Ferma il movimento //////////////////////////////////////////////////////////////////////////////////////
            traj_count = 0;
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
        } */
    }

    public void UpdateTrajectory (Vector2[] new_trajectory) // Function to set the new robot trajectory from the tab appp
    {
        trajectory = new_trajectory;
        traj_count = 0;
        SetNewtarget_position(trajectory[traj_count]);

        isMoving = true;
    }

    private void SetNewtarget_position(Vector2 newPos) /////////////// probabile prende byte in input ////////
    {
        target_position = new Vector3 (newPos.x, transform.position.y, newPos.y);
        // return target_position;
    }

    /* private Vector3 SetNewtarget_positionRandom()
    {
        // Genera una nuova posizione casuale all'interno del raggio
        float x = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);
        target_position = new Vector3(centerPoint.position.x + x, transform.position.y, centerPoint.position.z + z);
        return target_position;
    } */

    // Funzione per far girare il robot verso la main camera
    /* private void RotateTowardsMainCamera()
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
    } */

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

    // private Vector2[] readTxtToVector2List (string filename, int size)
    // {
    //     string[] lines = File.ReadAllLines(filename);
    //     List<Vector2> path = new List<Vector2>();

    //     try
    //     {
    //         foreach (string line in lines)
    //         {
    //             string[] parts = line.Split(new char[] { ','}, System.StringSplitOptions.RemoveEmptyEntries);

    //             if (parts.Length == 2) // ensure 2 values exist
    //             {
    //                 float x
    //             }
    //         }
    //     }
    //     catch 
    //     {
    //         Debug.Log("Error - Read txt file unsuccessful");
    //     }

    //     return path;
    // }
}
