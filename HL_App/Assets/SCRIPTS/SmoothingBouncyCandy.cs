/// <summary>
/// This class is responsible for creating a smooth bouncing effect for a GameObject.
/// It uses sine wave calculations to simulate the bounce effect by adjusting the GameObject's Y position.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothingBouncyCandy : MonoBehaviour
{
    private Vector3 initialPosition;
    public float bounceAmplitude = 0.1f; // Amplitude of the bounce
    public float bounceFrequency = 1f; // Frequency of the bounce

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the GameObject
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new Y position using a sine wave for smooth bouncing
        float newY = initialPosition.y + bounceAmplitude * Mathf.Sin(Time.time * bounceFrequency);
        // Update the position of the GameObject
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}