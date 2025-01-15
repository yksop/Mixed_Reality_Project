/// <summary>
/// The RobotController class is responsible for controlling the movement and behavior of a robot character in a Unity scene
/// by following the capsule.
/// It handles movement towards a target position, obstacle avoidance using SLAM, and animations based on the robot's state.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public float moveSpeed = 1f; // Movement speed

    public float distanceFromPlayer = 0.5f; // Distance at which the robot gets scared of the player

    public Animator animator;
    public bool isMoving = true; // Controls if the robot is moving

    // PID CONTROLLER
    [SerializeField] private PIDController z_controller; // Controller for forward direction of character
    [SerializeField] private PIDController yaw_controller; // Controller for yaw rotation (forward direction) of character
    [SerializeField] private Vector3 target_position; // Target destination in play-space

    [SerializeField] private bool is_PID_control;
    private bool facing_target;
    private bool at_target;

    private Vector2[] trajectory;
    private int traj_count = 0;

    Renderer[] characterMaterials;

    public AudioSource footsteps;

    public LayerMask spatialAwareness; // Ensure that the SLAM Layer is assigned to this variable

    public GameObject capsule;
    public CapsuleMovement capsuleMovement;

    public bool isManouvering = false;
    public Camera mainCamera;
    public Renderer avatar_color; // Reference to the avatar color renderer
    private Color initialColor; // Variable to store the initial color

    void Start()
    {
        // Get the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
            return;
        }

        // Initialize characterMaterials
        characterMaterials = GetComponentsInChildren<Renderer>(); // Fetch all child Renderers, adjust as needed

        //ChangeEyeOffset(EyePosition.normal);
        footsteps.Stop();

        // Initialize the robot in a stationary state
        animator.SetBool("isWalking", false);
        animator.SetBool("isFloating", false);
        //staCombattendo = false;

        mainCamera = Camera.main;
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

        target_position = capsule.transform.position; // Set the target position to the capsule's position
        // Save the initial color at startup
        initialColor = avatar_color.materials[1].color;
    }

    void Update()
    {
        // Set target_position to the capsule's position
        target_position = capsule.transform.position;

        if (isMoving)
        {
            MoveRobot(target_position); // Move the robot towards its target position

            // Calculate the direction towards the target position
            Vector3 targetDirection = (target_position - transform.position).normalized;
            targetDirection.y = 0; // Ignore the Y component to keep the robot on the horizontal plane

            // Calculate the required rotation
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Apply the rotation to the robot smoothly
            float rotationSpeed = 5f; // Rotation speed in seconds
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetMovingFalse()
    {
        isMoving = false;
    }

    public void OnHappyButtonPress()
    {
        isMoving = false;
        animator.SetTrigger("isHappy"); // Trigger the happy animation
        //ChangeEyeOffset(EyePosition.happy);
    }

    // Function to move the robot towards the target position
    private void MoveRobot(Vector3 target_position)
    {
        //ChangeEyeOffset(EyePosition.normal);
        // Play the footsteps sound only if it is not already playing
        if (!footsteps.isPlaying)
        {
            footsteps.Play();
        }
        // Box dimensions for the BoxCast (rectangular in the x-y plane)
        Vector3 boxSize = new Vector3(0.002f, 0.05f, 0.002f); // Adjust these dimensions based on the desired width and height

        // Calculate the direction towards the target position
        Vector3 direction = (target_position - transform.position).normalized;

        if (isManouvering == false)
        {
            // Use a BoxCast on the SLAM Mesh Layer to detect obstacles
            if (Physics.BoxCast(transform.position, boxSize / 2, direction, out RaycastHit hit, Quaternion.identity, moveSpeed * Time.deltaTime + 0.1f, spatialAwareness))
            {
                // If the hit object has the SLAM Mesh Layer, stop the movement
                Debug.Log("Hit SLAMMesh object, stopping movement.");
                capsuleMovement.StopMovement();
                //target_position = transform.position;
                animator.SetBool("isWalking", false);
                footsteps.Stop();
                target_position = transform.position;
                //return;
            }
        }

        float avatar_vel = moveSpeed;

        Vector2 player = new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.z);
        Vector2 avatar = new Vector2(transform.position.x, transform.position.z);
        float dist_2_user = Vector2.Distance(player, avatar);

        if (dist_2_user > distanceFromPlayer)
        {
            avatar_vel = avatar_vel / Mathf.Pow(dist_2_user - distanceFromPlayer, 4);
            if (avatar_vel > 1.25f * moveSpeed)
            {
                avatar_vel = 1.25f * moveSpeed;
            }
            else if (isManouvering == true)
            {
                avatar_vel = moveSpeed;
            }
            capsuleMovement.avatar_vel = avatar_vel;
        }

        // Implementing PID controller to move and rotate player
        // First transform target position into character's local coordinates
        target_position.y = transform.position.y; // We do not implement any control in the vertical direction (i.e. only gravity)

        Vector3 target_direction = (target_position - transform.position).normalized;

        var current_angle = Vector3.SignedAngle(Vector3.forward, transform.forward, Vector3.up); // Character argan angle
        var target_angle = Vector3.SignedAngle(Vector3.forward, target_direction, Vector3.up); // Target argan angle
        float input_yaw = yaw_controller.UpdateAngle(Time.fixedDeltaTime, current_angle, target_angle); // yaw_controller update

        facing_target = Mathf.Abs(input_yaw) < yaw_controller.threshold ? true : false;

        float distance = (target_position - transform.position).magnitude;
        float character_vel = moveSpeed;
        //float anim_blend = 0.5f;

        float input_z = z_controller.UpdatePosition(Time.fixedDeltaTime, transform.position.z, transform.position.z + distance);

        at_target = Mathf.Abs(input_z) < z_controller.threshold ? true : false;

        var desired_move_direction = new Vector3(0, 0, input_z);
        desired_move_direction = transform.TransformVector(desired_move_direction);

        //transform.Translate ( desired_move_direction * Time.deltaTime * character_vel, Space.World ); // Robot walks to set target
        transform.Translate(desired_move_direction * Time.deltaTime * avatar_vel, Space.World); // Robot walks to set target
        // anim.SetFloat("Blend", anim_blend, StartAnimTime, Time.deltaTime);

        if (isManouvering == true)
        {
            animator.SetBool("isFloating", true);
            avatar_color.materials[1].color = new Color32(0x43, 0xFF, 0x00, 0xFF);
        }
        else
        {
            animator.SetBool("isFloating", false);
            animator.SetBool("isWalking", true);
            avatar_color.materials[1].color = initialColor; // Return to the initial color
        }
    }

    public void UpdateTrajectory(Vector2[] new_trajectory) // Function to set the new robot trajectory from the tab app
    {
        trajectory = new_trajectory;
        traj_count = 0;
        SetNewtarget_position(trajectory[traj_count]);

        isMoving = true;
    }

    private void SetNewtarget_position(Vector2 newPos) /////////////// probably takes byte as input ////////
    {
        target_position = new Vector3(newPos.x, transform.position.y, newPos.y);
        // return target_position;
    }
}