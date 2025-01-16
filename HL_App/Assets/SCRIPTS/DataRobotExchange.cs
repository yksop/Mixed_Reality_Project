using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using M2MqttUnity;
using System.Text;

/// <summary>
/// The DataRobotExchange class is responsible for managing the interaction between a robot, a player, and the environment.
/// It calculates and sends the positions and orientations of the robot and player relative to a central point.
/// Additionally, it performs periodic spatial awareness checks using raycasts to detect obstacles and sends the impact points.
/// </summary>
public class DataRobotExchange : MonoBehaviour
{
    public GameObject Robottino; // Reference to the robot
    public GameObject Center;    // Reference to the central point
    public GameObject Player;    // Reference to the player
    public float maxDistance = 20f; // Maximum distance for the raycast
    private int layerMask; // Layer mask for the raycast
    public int rayPerDegree = 1;
    public float lowRayHeight = 0.3f;
    public float highRayHeight = 2f;
    public float boxWidth = 0.005f;
    public float boxHeight = 0.1f;
    private string h;
    public float bubble = 1f; // Radius of the bubble to be ignored around the player

    public BaseClient baseClient;

    private void Start()
    {
        // Set the layer mask for the "SpatialAwareness" layer
        layerMask = LayerMask.GetMask("SpatialAwareness");
        StartCoroutine(GetSpatialPoints());
        Debug.Log("Start");
    }
    
    private void FixedUpdate()
    {
        // Send the position and rotation of the robot and player relative to the center
        baseClient.SendPosRot(Robottino, Robottino.transform.position - Center.transform.position, Robottino.transform.rotation);
        baseClient.SendPosRot(Player, Player.transform.position - Center.transform.position, Player.transform.rotation);
    }

    // Function that calculates the position of the robot relative to the center and saves the values in a byte array
    public byte[] CalcolaPosizione()
    {
        // Get the relative position of the robot to the center only on the X,Z plane
        Vector3 posizioneRelativa = Robottino.transform.position - Center.transform.position;

        // Byte list to store only X and Z
        List<byte> byteArray = new List<byte>();

        // Convert the X coordinate
        byteArray.AddRange(BitConverter.GetBytes(posizioneRelativa.x));

        // Convert the Z coordinate
        byteArray.AddRange(BitConverter.GetBytes(posizioneRelativa.z));

        // Return the byte array with only X and Z
        return byteArray.ToArray();
    }

    public byte[] CalcolaDirezioneGuardataEQuaternion()
    {
        // "Forward" direction of the robot (where it is looking), normalized on the X,Z plane
        Vector3 direzioneGuardata = new Vector3(Robottino.transform.forward.x, 0, Robottino.transform.forward.z).normalized;

        // Rotation of the robot in quaternion
        Quaternion rotazioneGuardata = Robottino.transform.rotation;

        // Byte list to store data
        List<byte> byteArray = new List<byte>();

        // Convert the X coordinate of the direction looked at
        byteArray.AddRange(BitConverter.GetBytes(direzioneGuardata.x));

        // Convert the Z coordinate of the direction looked at
        byteArray.AddRange(BitConverter.GetBytes(direzioneGuardata.z));

        // Convert the quaternion components (x, y, z, w)
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.x));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.y));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.z));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.w));

        // Return the byte array with X, Z of the direction looked at and the quaternion
        return byteArray.ToArray();
    }

    public IEnumerator GetSpatialPoints()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            Debug.Log("Calculating impact points");
            CalcolaPuntiImpattoCircolari(highRayHeight + Center.transform.position.y);
            CalcolaPuntiImpattoCircolari(lowRayHeight + Center.transform.position.y);
        }
    }

    // Function to calculate the impact points of the raycasts in a byte array
    public void CalcolaPuntiImpattoCircolari(float height)
    {
        List<byte> puntiImpattoBytes = new List<byte>();

        // Starting position of the raycast
        Vector3 startPosition;

        // Angular resolution of 1 degree to cover 360 degrees
        int stepDegrees = rayPerDegree;

        // Boxcast: Initial position of box (center of player) and dimensions
        Vector3 boxCenter;
        Vector3 boxHalfExtents = new Vector3(boxWidth, boxHeight, boxWidth);

        for (int angle = 0; angle < 360; angle += stepDegrees)
        {
            // Calculate the direction of the raycast for the current angle (on the X,Z plane)
            float rad = Mathf.Deg2Rad * angle;
            startPosition = new Vector3(Player.transform.position.x + bubble * Mathf.Cos(rad), height, Player.transform.position.z + bubble * Mathf.Sin(rad));
            boxCenter = startPosition;
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

            // Perform the raycast from the elevated position in this horizontal direction
            if (Physics.BoxCast(boxCenter, boxHalfExtents, direction, out RaycastHit hit, Quaternion.identity, maxDistance, layerMask))
            {
                // Get the impact point relative to the Vuforia marker
                Vector3 puntoImpatto = hit.point - Center.transform.position;

                puntiImpattoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.x));
                puntiImpattoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.z));
            }
        }

        if (height == lowRayHeight + Center.transform.position.y)
        {
            h = "low";
        }
        else
        {
            h = "high";
        }
        baseClient.SendRoom(h, puntiImpattoBytes.ToArray());
    }

    // Function to change the height of the low boxcast height
    public void ChangeLowHeight(float num)
    {
        lowRayHeight = num;
    }

    // Function to change the height of the high boxcast height
    public void ChangeHighHeight(float num)
    {
        highRayHeight = num;
    }

    // Function to change the radius of the bubble around the player to ignore boxcast
    public void ChangeBubble(float num)
    {
        bubble = num;
    }
}