using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject MainCanvas;  // Riferimento alla Canvas principale
    public GameObject GraphCanvas; // Riferimento alla Canvas dei grafici

    // Mostra la Canvas dei grafici
    public void ShowGraphCanvas()
    {
        MainCanvas.SetActive(false);  // Nascondi la Canvas principale
        GraphCanvas.SetActive(true); // Mostra la Canvas dei grafici
    }

    // Torna alla Canvas principale
    public void ShowMainCanvas()
    {
        GraphCanvas.SetActive(false); // Nascondi la Canvas dei grafici
        MainCanvas.SetActive(true);  // Mostra la Canvas principale
    }
}