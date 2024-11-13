using UnityEngine;
using Vuforia;
using UnityEngine.XR.ARFoundation;

public class AnchorManager : MonoBehaviour
{
    private TrackableBehaviour trackableBehaviour; // Usato per gestire l'oggetto tracciato da Vuforia
    public ARAnchorManager anchorManager; // Manager per la gestione delle ancore
    private ARAnchor anchor; // Variabile per l'ancora spaziale
    private bool anchorSet = false;
    private bool vuforiaDisabled = false;

    void Start()
    {
        // Ottieni il componente TrackableBehaviour di Vuforia
        trackableBehaviour = GetComponent<TrackableBehaviour>();
        EnableVuforiaManually();
    }

    // Funzione chiamata quando premi il bottone per impostare l'ancora
    public void SetSpatialAnchorManually()
    {
        if (!anchorSet && trackableBehaviour != null && trackableBehaviour.CurrentStatus == TrackableBehaviour.Status.TRACKED)
        {
            SetSpatialAnchor();
        }
    }

    // Funzione per impostare l'ancora spaziale
    private void SetSpatialAnchor()
    {
        // Ottieni la posizione e la rotazione dell'immagine rilevata da Vuforia
        Vector3 position = trackableBehaviour.transform.position;
        Quaternion rotation = trackableBehaviour.transform.rotation;

        // Crea un nuovo GameObject per l'ancora
        GameObject anchorObject = new GameObject("SpatialAnchor");

        // Posiziona l'oggetto nella posizione rilevata
        anchorObject.transform.position = position;
        anchorObject.transform.rotation = rotation;

        // Crea un Pose a partire dalla posizione e rotazione
        Pose anchorPose = new Pose(anchorObject.transform.position, anchorObject.transform.rotation);

        // Aggiungi l'oggetto al sistema di ancore spaziali usando il Pose
        anchor = anchorManager.AddAnchor(anchorPose);

        // Conferma che l'ancora è stata impostata
        if (anchor != null)
        {
            Debug.Log("Ancora spaziale posizionata correttamente.");
            anchorSet = true; // Imposta il flag per evitare altre ancore
        }
        else
        {
            Debug.LogError("Errore durante la creazione dell'ancora spaziale.");
        }
    }

    // Funzione chiamata quando premi il bottone per disattivare Vuforia
    public void DisableVuforiaManually()
    {
        if (!vuforiaDisabled)
        {
            VuforiaBehaviour.Instance.enabled = false; // Disabilita Vuforia
            vuforiaDisabled = true;
            Debug.Log("Vuforia disabilitato.");
        }
    }

    // Funzione per riattivare Vuforia, se necessario
    public void EnableVuforiaManually()
    {
        if (vuforiaDisabled)
        {
            VuforiaBehaviour.Instance.enabled = true; // Riattiva Vuforia
            vuforiaDisabled = false;
            Debug.Log("Vuforia riattivato.");
        }
    }
}
