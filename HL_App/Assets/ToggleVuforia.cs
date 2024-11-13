using UnityEngine;
using Vuforia;
using TMPro; // Assicurati di includere il namespace di TextMeshPro

public class ToggleVuforia : MonoBehaviour
{
    // Variabile per tenere traccia se Vuforia è attivo o meno
    private bool isVuforiaActive = true;
    public TextMesh testo; // Riferimento al componente TextMeshPro
    public TextMeshProUGUI testoBottone; // Riferimento al componente TextMeshPro

    public Canvas canvasBottoniRobot;

    // Riferimento al VuforiaBehaviour
    private VuforiaBehaviour vuforiaBehaviour;

    void Start()
    {
        // Ottieni il componente VuforiaBehaviour alla partenza
        vuforiaBehaviour = FindObjectOfType<VuforiaBehaviour>();
        
        // Assicurati che Vuforia sia attivo inizialmente (se vuoi)
        if (vuforiaBehaviour != null)
        {
            vuforiaBehaviour.enabled = true;
            testo.text = "On-init"; // Imposta il testo iniziale su "On"
            testoBottone.text = "Vuforia On"; // Imposta il testo iniziale su "On"
            
            // Riattiva il tracciamento se Vuforia è stato abilitato
            TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
            canvasBottoniRobot.gameObject.SetActive(true); // Disabilita il canvas

        }
    }

    // Funzione che verrà chiamata dal bottone per attivare/disattivare Vuforia
    public void ToggleVuforiaTracking()
    {
        if (vuforiaBehaviour != null)
        {
            // Alterna lo stato di abilitazione di Vuforia
            isVuforiaActive = !isVuforiaActive;
            vuforiaBehaviour.enabled = isVuforiaActive;

            // Se Vuforia è disabilitato, si ferma il tracciamento degli oggetti
            if (!isVuforiaActive)
            {
                TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
                testoBottone.text = "Vuforia Off"; // Imposta il testo iniziale su "On"
                testo.text = "Off"; // Aggiorna il testo su "Off"
                canvasBottoniRobot.gameObject.SetActive(false); // Disabilita il canvas
            }
            else
            {
                // Riattiva il tracciamento se Vuforia è stato abilitato
                TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
                testo.text = "On"; // Aggiorna il testo su "On"
                testoBottone.text = "Vuforia On";
                canvasBottoniRobot.gameObject.SetActive(true); // Disabilita il canvas
            }
        }
    }
}
