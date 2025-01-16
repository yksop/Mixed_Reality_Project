/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPathGenerator : MonoBehaviour
{
    // Reference to the robot controller script
    public RobotController robotController;
    public CapsuleMovement capsuleMovement;

    // Number of points to generate along the S path
    public int pointCount = 50;

    // Dimensions of the S-curve
    public float curveWidth = 10f;   // Horizontal span of the S
    public float curveHeight = 5f;   // Vertical height of the S

    // The generated trajectory
    private Vector2[] trajectoryPoints;

    void Start()
    {
        // Automatically find RobotController if not assigned
        if (robotController == null)
        {
            robotController = GetComponent<RobotController>();
        }

        // Start the coroutine to send the path after 5 seconds
        StartCoroutine(SendPathAfterDelay(5f));
    }

    /// <summary>
    /// Coroutine to send trajectory after a delay
    /// </summary>
    private IEnumerator SendPathAfterDelay(float delay)
    {
        Debug.Log($"Waiting {delay} seconds to send the trajectory...");

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Generate and send the trajectory
        GenerateSTrajectory();

        if (robotController != null && capsuleMovement != null)
        {
            // robotController.UpdateTrajectory(trajectoryPoints);
            capsuleMovement.SetTrajectory(trajectoryPoints);
            Debug.Log("Trajectory sent to RobotController after delay.");
        }
        else
        {
            Debug.LogError("RobotController not assigned!");
        }

        Debug.Log($"Waiting {delay} seconds to send the trajectory...");

        // Wait for the specified delay
        yield return new WaitForSeconds(delay+15);

        // Generate and send the trajectory
        GenerateCTrajectory();

        if (robotController != null && capsuleMovement != null)
        {
            // robotController.UpdateTrajectory(trajectoryPoints);
            capsuleMovement.SetTrajectory(trajectoryPoints);
            Debug.Log("Trajectory sent to RobotController after delay.");
        }
        else
        {
            Debug.LogError("RobotController not assigned!");
        }
    }

    /// <summary>
    /// Generates points along an S-shaped curve and stores them in trajectoryPoints.
    /// </summary>
    private void GenerateSTrajectory()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            // t ranges from -1 to 1 for the S curve
            float t = -1f + 2f * (i / (float)(pointCount - 1));

            // Parametric equation for the S-curve
            float x = curveWidth * t;
            float y = curveHeight * Mathf.Sin(Mathf.PI * t);

            points.Add(new Vector2(x, y));
        }

        // Convert List to an array for the robot controller
        trajectoryPoints = points.ToArray();

        Debug.Log("S-Trajectory Generated with " + trajectoryPoints.Length + " points.");
    }

    private void GenerateCTrajectory()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < pointCount; i++)
        {
            // t ranges from -1 to 1 for the S curve
            float t = -1f + 2f * (i / (float)(pointCount - 1));

            // Parametric equation for the S-curve
            float x = curveWidth * t;
            float y = curveHeight * Mathf.Cos(Mathf.PI * t);

            points.Add(new Vector2(x, y));
        }

        // Convert List to an array for the robot controller
        trajectoryPoints = points.ToArray();

        Debug.Log("S-Trajectory Generated with " + trajectoryPoints.Length + " points.");
    }

}
 */