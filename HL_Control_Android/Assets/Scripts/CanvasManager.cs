/// <summary>
/// The CanvasManager class is responsible for managing the visibility of different canvases in the application.
/// It allows switching between the main canvas and the graph canvas, and interacts with the RetrieveData class
/// to reset graph data when returning to the main canvas.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject MainCanvas;  // Reference to the main Canvas
    public GameObject GraphCanvas; // Reference to the graph Canvas

    public RetrieveData retrieveData; // Reference to the RetrieveData class

    // Show the graph Canvas
    public void ShowGraphCanvas()
    {
        MainCanvas.SetActive(false);  // Hide the main Canvas
        GraphCanvas.SetActive(true); // Show the graph Canvas
    }

    // Return to the main Canvas
    public void ShowMainCanvas() // when click back button
    {
        GraphCanvas.SetActive(false); // Hide the graph Canvas
        MainCanvas.SetActive(true);  // Show the main Canvas
        retrieveData.ResetGraphData(false); // Reset graph data in RetrieveData but not the Data  
    }
}