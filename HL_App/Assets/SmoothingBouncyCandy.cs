using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothingBouncyCandy : MonoBehaviour
{
    private Vector3 initialPosition;
    public float bounceAmplitude = 0.1f; // Ampiezza del movimento
    public float bounceFrequency = 1f; // Frequenza del movimento

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = initialPosition.y + bounceAmplitude * Mathf.Sin(Time.time * bounceFrequency);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}