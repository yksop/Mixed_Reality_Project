using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.WindowsDevicePortal;
using UnityEngine;

/// <summary>
/// The <c>CapsuleMovement</c> class manages the movement of a capsule along a predefined trajectory.
/// The capsule moves towards a series of points specified in an array of <c>Vector2</c>.
/// The movement of the capsule is controlled by a PID controller that adjusts the robot's speed to follow the capsule.
/// </summary>
public class CapsuleMovement : MonoBehaviour
{
    // Array of points representing the trajectory
    private Vector2[] trajectory;
    private int currentPointIndex = 0;
    public float baseSpeed = 1.0f;
    // Target position towards which the capsule will move
    private Vector3 targetPosition;
    public GameObject avatar;
    public RobotController robotController;
    public DroppingCandies playerCandies;
    public float distanceFromRobot = 0.1f; 
    public float rotationSpeed = 5.0f;
    public float avatar_vel = 0f;

    void Start()
    {
        //Debug.Log("CapsuleMovement started.");
    }

    void Update()
    {
        // Check if the trajectory has been set and if there are still points to reach
        if (trajectory != null && currentPointIndex < trajectory.Length)
        {
            // Set the target position to the current point of the trajectory
            targetPosition = new Vector3(trajectory[currentPointIndex].x , transform.position.y, trajectory[currentPointIndex].y);
            // Move the capsule towards the target position
            MoveCapsule(targetPosition);
            
            if(currentPointIndex == 0)
            {
                robotController.isManouvering = true;
            }else
            {
                robotController.isManouvering = false;
            }

            // Check if the capsule has reached the target position
            if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPosition.x, targetPosition.z)) <= 0.05)
            {
                // Increment the current point index
                currentPointIndex++;
                //Debug.Log("Reached point " + currentPointIndex);
            }
            //Debug.Log(trajectory[0].ToString());
        }
        else
        {
            //Debug.Log("Trajectory is null or completed.");
            // Check if the capsule has reached the avatar on the x,z plane
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(avatar.transform.position.x, avatar.transform.position.z)) <= distanceFromRobot)
            {
                //Debug.Log("Reached avatar.");
                robotController.isManouvering = false;
                robotController.isMoving = false;
                robotController.animator.SetBool("isWalking", false);
                robotController.animator.SetBool("isFloating", false);
            }
        }
    }

    // Method to set a new trajectory
    public void SetTrajectory(Vector2[] newTrajectory)
    {
        // Set the new trajectory
        trajectory = newTrajectory;
        if (newTrajectory == null)
        {
            Debug.Log("Trajectory is null.");
        }
        robotController.isManouvering = true;

        // Reset the current point index
        currentPointIndex = 0;
        // If the robot controller is assigned
        if (robotController != null)
        {
            // Set the robot in motion
            robotController.isMoving = true;
            // Destroy all candies
            playerCandies.DestroyAllCandies();
            //playerCandies.takeDonutCounter = 0;
        }
        Debug.Log("Trajectory set with " + trajectory.Length + " points.");
    }

    // Method to move the capsule towards a target position
    private void MoveCapsule(Vector3 targetPosition)
    {
        float distanceToAvatar = Vector3.Distance(transform.position, avatar.transform.position);
        float speed = Mathf.Max(baseSpeed / Mathf.Pow(distanceToAvatar, 4), avatar_vel);
        
        // Calculate the direction towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        // Rotate the capsule towards the target position
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //Debug.Log("Moving towards " + targetPosition + " with speed " + speed);
    }

    // Method to stop the movement of the capsule
    public void StopMovement()
    {
        trajectory = null;
        currentPointIndex = 0;
        robotController.isMoving = false;
        //Debug.Log("Movement stopped.");
    }
}