/* using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class RobotInteraction : MonoBehaviour
{
    // Riferimento all'Animator
    private Animator animator;

    // Nome del parametro Trigger nell'Animator
    public string triggerFightName = "inFight";

    // Riferimento allo script RobotMoveOnButtonPress
    private RobotMoveOnButtonPress robotMover;

    void Start()
    {
        // Ottieni il componente Animator dal robot
        animator = GetComponent<Animator>();

        // Controlla se l'Animator è presente
        if (animator == null)
        {
            Debug.LogError("Animator non trovato! Assicurati che questo GameObject abbia un componente Animator.");
        }

        // Ottieni il riferimento a RobotMoveOnButtonPress
        robotMover = GetComponent<RobotMoveOnButtonPress>();
        if (robotMover == null)
        {
            Debug.LogWarning("RobotMoveOnButtonPress non trovato! Assicurati che sia attaccato allo stesso GameObject o modifica il riferimento.");
        }
    }

    // Metodo chiamato quando l'oggetto viene toccato
    public void OnTouch()
    {
        if (animator != null)
        {
            // Attiva il trigger nell'Animator
            animator.SetTrigger(triggerFightName);
            Debug.Log("Trigger attivato: " + triggerFightName);
        }

        // Disattiva RobotMoveOnButtonPress
        if (robotMover != null)
        {
            //robotMover.enabled = false;
            Debug.Log("RobotMoveOnButtonPress disattivato.");
        }

        RotateTowardsMainCamera();
    }


    // Metodo per far girare il robot verso la main camera
    private void RotateTowardsMainCamera()
    {
        // Ottieni la posizione della main camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
         // Calcola la direzione verso la camera
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        directionToCamera.y = 0; // Ignora la componente y per mantenere il robot sul piano orizzontale

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
}
 */