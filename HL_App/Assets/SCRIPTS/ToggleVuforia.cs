using UnityEngine;
using Vuforia;
using TMPro; // Make sure to include the TextMeshPro namespace

/// <summary>
/// This class is responsible for toggling the Vuforia AR functionality on and off.
/// It manages the VuforiaBehaviour component and updates the UI text elements accordingly.
/// </summary>
public class ToggleVuforia : MonoBehaviour
{
    // Variable to keep track of whether Vuforia is active or not
    private bool isVuforiaActive = true;
    public TextMesh text; // Reference to the TextMeshPro component
    public TextMeshProUGUI buttonText; // Reference to the TextMeshPro component

    //public Canvas robotButtonsCanvas;

    // Reference to the VuforiaBehaviour
    private VuforiaBehaviour vuforiaBehaviour;

    void Start()
    {
        // Get the VuforiaBehaviour component at the start
        vuforiaBehaviour = FindObjectOfType<VuforiaBehaviour>();
        
        // Ensure Vuforia is active initially (if desired)
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = true;
            text.text = "On-init"; // Set the initial text to "On"
            buttonText.text = "Vuforia On"; // Set the initial button text to "On"
            
            // Reactivate tracking if Vuforia has been enabled
            TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
            //robotButtonsCanvas.gameObject.SetActive(true); // Enable the canvas

        }
    }

    // Function that will be called by the button to toggle Vuforia
    public void ToggleVuforiaTracking()
    {
        if (vuforiaBehaviour != null)
        {
            // Toggle the enabled state of Vuforia
            isVuforiaActive = !isVuforiaActive;
            vuforiaBehaviour.enabled = isVuforiaActive;

            // If Vuforia is disabled, stop object tracking
            if (!isVuforiaActive)
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
                buttonText.text = "Vuforia Off"; // Set the button text to "Off"
                text.text = "Off"; // Update the text to "Off"
                //robotButtonsCanvas.gameObject.SetActive(false); // Disable the canvas
            }
            else
            {
                // Reactivate tracking if Vuforia has been enabled
                TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
                text.text = "On"; // Update the text to "On"
                buttonText.text = "Vuforia On";
                //robotButtonsCanvas.gameObject.SetActive(true); // Enable the canvas
            }
        }
    }
}
