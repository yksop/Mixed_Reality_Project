using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    // Array di punti che rappresentano la traiettoria
    private Vector2[] trajectory;
    private int currentPointIndex = 0;
    public float baseSpeed = 1.0f;
    // Posizione target verso cui la capsula si muoverà
    private Vector3 targetPosition;
    public GameObject avatar;
    public RobotController robotController;
    public DroppingCandies playerCandies;

    void Start()
    {
        //Debug.Log("CapsuleMovement started.");
    }

    void Update()
    {
        // Controlla se la traiettoria è stata impostata e se ci sono ancora punti da raggiungere
        if (trajectory != null && currentPointIndex < trajectory.Length)
        {
            // Imposta la posizione target al punto corrente della traiettoria
            targetPosition = new Vector3(trajectory[currentPointIndex].x, transform.position.y, trajectory[currentPointIndex].y);
            // Muove la capsula verso la posizione target
            MoveCapsule(targetPosition);

            // Controlla se la capsula ha raggiunto la posizione target
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Incrementa l'indice del punto corrente
                currentPointIndex++;
                //Debug.Log("Reached point " + currentPointIndex);
            }
        }
        else
        {
            //Debug.Log("Trajectory is null or completed.");
            // Controlla se la capsula ha raggiunto l'avatar nel piano x,z
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(avatar.transform.position.x, avatar.transform.position.z)) <= 0.5f)
            {
                //Debug.Log("Reached avatar.");
                robotController.isMoving = false;
                robotController.animator.SetBool("isWalking", false);
            }
        }
    }

    // Metodo per impostare una nuova traiettoria
    public void SetTrajectory(Vector2[] newTrajectory)
    {
        // Imposta la nuova traiettoria
        trajectory = newTrajectory;
        // Resetta l'indice del punto corrente
        currentPointIndex = 0;
        // Se il controller del robot è assegnato
        if (robotController != null)
        {
            // Imposta il robot in movimento
            robotController.isMoving = true;
            // Distrugge tutte le caramelle
            playerCandies.DestroyAllCandies();
            //playerCandies.takeDonutCounter = 0;
        }
        Debug.Log("Trajectory set with " + trajectory.Length + " points.");
    }

    // Metodo per muovere la capsula verso una posizione target
    private void MoveCapsule(Vector3 targetPosition)
    {
        float distanceToAvatar = Vector3.Distance(transform.position, avatar.transform.position);
        float speed = baseSpeed / Mathf.Pow(distanceToAvatar, 4);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //Debug.Log("Moving towards " + targetPosition + " with speed " + speed);
    }

    // Metodo per fermare il movimento della capsula
    public void StopMovement()
    {
        trajectory = null;
        currentPointIndex = 0;
        robotController.isMoving = false;
        //Debug.Log("Movement stopped.");
    }
}