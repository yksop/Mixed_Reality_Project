using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    private Vector2[] trajectory;
    private int currentPointIndex = 0;
    public float baseSpeed = 1.0f;
    private Vector3 targetPosition;
    public GameObject avatar;
    public RobotController robotController;

    public DroppingCandies playerCandies;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("CapsuleMovement started.");
    }

    // Update is called once per frame
    void Update()
    {

        if (trajectory != null && currentPointIndex < trajectory.Length)
        {
            targetPosition = new Vector3(trajectory[currentPointIndex].x, transform.position.y, trajectory[currentPointIndex].y);
            MoveCapsule(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPointIndex++;
                Debug.Log("Reached point " + currentPointIndex);
            }
        }
        else
        {
            //Debug.Log("Trajectory is null or completed.");
            if ( Vector3.Distance(transform.position, avatar.transform.position) < 0.1f)
            {
                Debug.Log("Reached avatar.");
                //robotController.isMoving = false;
                //robotController.animator.SetBool("isWalking", false);
            }
        }
    }

    public void SetTrajectory(Vector2[] newTrajectory)
    {
        trajectory = newTrajectory;
        currentPointIndex = 0;
        if (robotController != null)
        {
            robotController.isMoving = true;
            playerCandies.DestroyAllCandies();
            //playerCandies.takeDonutCounter = 0;
        }
        Debug.Log("Trajectory set with " + trajectory.Length + " points.");
    }

    private void MoveCapsule(Vector3 targetPosition)
    {
        float distanceToAvatar = Vector3.Distance(transform.position, avatar.transform.position);
        float speed = baseSpeed / Mathf.Pow(distanceToAvatar, 4);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //Debug.Log("Moving towards " + targetPosition + " with speed " + speed);
    }

    public void StopMovement()
    {
        trajectory = null;
        currentPointIndex = 0;
        robotController.isMoving = false;
        //Debug.Log("Movement stopped.");
    }
}