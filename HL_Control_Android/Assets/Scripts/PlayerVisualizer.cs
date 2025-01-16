using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for visualizing the position/rotation of the player and the avatar updated from the Holo Lens app through he Base Client
/// </summary>
public class PlayerVisualizer : MonoBehaviour
{
    Vector3 position; // Stores the position of the player
    Quaternion rotation; // Stores the rotation of the player
    public GameObject player; // Reference to the player GameObject
    public GameObject avatar; // Reference to the avatar GameObject
    public GameObject center; // Reference to the center GameObject
    public float movemetMultiplier = 100f; // Multiplier for movement scaling
    //public Material lineMaterial;
    private Color lineColor = Color.green; // Color for line rendering

    // Method to update the player's position
    public void PlayerUpdatePosition(byte[] posB)
    {
        Vector3 pos = BytesToVector3(posB); // Convert byte array to Vector3
        player.transform.position = center.transform.position + pos * movemetMultiplier; // Update player's position
    }

    // Method to update the player's rotation
    public void PlayerUpdateRotation(byte[] rotB)
    {
        Quaternion rot = BytesToQuaternion(rotB); // Convert byte array to Quaternion
        player.transform.eulerAngles = new Vector3(0f, 0f, -rot.eulerAngles.y); // Update player's rotation
    }

    // Method to update the avatar's position
    public void AvatarUpdatePosition(byte[] posB)
    {
        Vector3 pos = BytesToVector3(posB); // Convert byte array to Vector3
        avatar.transform.position = center.transform.position + pos * movemetMultiplier; // Update avatar's position
    }

    // Method to update the avatar's rotation
    public void AvatarUpdateRotation(byte[] rotB)
    {
        Quaternion rot = BytesToQuaternion(rotB); // Convert byte array to Quaternion
        avatar.transform.eulerAngles = new Vector3(0f, 0f, -rot.eulerAngles.y); // Update avatar's rotation
    }

    // Method to convert byte array to Vector3
    private Vector3 BytesToVector3(byte[] bytes)
    {
        if (bytes.Length != 24)
        {
            throw new ArgumentException("Byte array length must be 24 bytes (3 doubles).");
        }

        double x = BitConverter.ToDouble(bytes, 0);
        double z = BitConverter.ToDouble(bytes, 8);
        double y = BitConverter.ToDouble(bytes, 16);

        return new Vector3((float)x, (float)y, (float)z);
    }

    // Method to convert byte array to Quaternion
    private Quaternion BytesToQuaternion(byte[] bytes)
    {
        if (bytes.Length != 32)
        {
            throw new ArgumentException("Byte array length must be 32 bytes (4 doubles).");
        }

        double a = BitConverter.ToDouble(bytes, 0);
        double b = BitConverter.ToDouble(bytes, 8);
        double c = BitConverter.ToDouble(bytes, 16);
        double d = BitConverter.ToDouble(bytes, 24);

        return new Quaternion((float)a, (float)b, (float)c, (float)d);
    }

    /*
    public void DrawRoom(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = points[i];
            LineRenderer lr = myLine.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr.material = lineMaterial;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, points[i]);
            lr.SetPosition(1, points[i+1]);
        }
    }
    public void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lr.material = lineMaterial;
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
    */
}