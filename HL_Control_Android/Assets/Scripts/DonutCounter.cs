using System;
using UnityEngine;
using TMPro;
using M2MqttUnity;
using System.Runtime.ConstrainedExecution;

///<summary>
/// This class is responsible for updating the donut counter and resetting it, sending a flag to the Holo Lens app throught Base Client to reset it
/// </summary>
public class DonutCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText; // Reference to the TextMeshProUGUI component to display the counter

    public BaseClient baseClient; // Reference to the BaseClient for MQTT communication
    private int counter = 0; // Counter to keep track of the number of donuts eaten
    private int total = 0; // Total amount of donuts dropped by the avatar
    private double per = 0;
    public DonutVisualizer donutVisualizer;

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
    public void UpdateCounter(int c, int t)
    {
        counter += c;
        total += t;
        // Check if the counterText is assigned
        if (counterText != null)
        {
            if (total != 0)
            {
                per = Math.Round((float)counter / (float)total * 100f);
            }
            else
            {
                per = 0;
            }
            double percent = Math.Round((float)counter / (float)total * 100f);
            counterText.text = "Donuts Eaten: " + counter.ToString() + "/" + total.ToString() + " (" + per + "%)"; // Update the counter text
        }
        else
        {
            Debug.LogError("TextMeshProUGUI not assigned!"); // Log an error if counterText is not assigned
        }
    }

    // Method to reset the counter
    public void ResetCounter()
    {
        // Reset the counter to 0
        int tot = donutVisualizer.donuts.Count;
        UpdateCounter(-counter, -(total-tot));
    }
}