using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    // Variabile pubblica per il GameObject da attivare/disattivare
    public GameObject targetObject;

    // Start è chiamato una volta, all'inizio
    void Start()
    {
        // Assicuriamoci che targetObject non sia nullo
        if (targetObject == null)
        {
            Debug.LogWarning("TargetObject non assegnato! Assegna un GameObject nell'Inspector.");
        }
    }

    // Metodo pubblico che fa il toggle del GameObject
    public void ToggleGameObject()
    {
        if (targetObject != null)
        {
            // Cambia lo stato attivo di targetObject
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
