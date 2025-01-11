using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using M2MqttUnity;

public class DonutCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText; // Reference to the TextMeshProUGUI component to display the counter

    private BaseClient baseClient; // Reference to the BaseClient for MQTT communication
    private int counter = 0; // Counter to keep track of the number of donuts eaten

    // Start is called before the first frame update
    void Start()
    {
        // Check if the counterText is assigned
        if (counterText == null)
        {
            Debug.LogError("TextMeshProUGUI not assigned!"); // Log an error if counterText is not assigned
        }
        counterText.text = "Donuts Eaten: 0"; // Initialize the counter text
    }

    // Method to update the counter display
    public void UpdateCounter(int counter)
    {
        // Check if the counterText is assigned
        if (counterText != null)
        {
            counterText.text = "Donuts Eaten: " + counter.ToString(); // Update the counter text
        }
        else
        {
            Debug.LogError("TextMeshProUGUI not assigned!"); // Log an error if counterText is not assigned
        }
    }

    // Method to reset the counter
    public void ResetCounter()
    {
        counter = 0; // Reset the counter to 0
        UpdateCounter(counter); // Update the counter display
        //baseClient.SendCounterReset(); // Uncomment this line to send a counter reset message via MQTT
    }
}