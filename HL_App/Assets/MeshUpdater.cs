using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;

public class MeshUpdater : MonoBehaviour
{
    private IMixedRealitySpatialAwarenessMeshObserver meshObserver;
    private MeshCollider meshCollider;
    private float updateInterval = 10.0f; // Intervallo di aggiornamento in secondi
    private float timer = 0.0f;

    void Start()
    {
        // Ottieni il componente MeshCollider dell'oggetto a cui questo script è assegnato
        meshCollider = GetComponent<MeshCollider>();

        // Ottieni l'osservatore di mesh spaziale dalle HoloLens
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        if (observer != null)
        {
            meshObserver = observer;
        }
        else
        {
            Debug.LogError("Spatial Awareness Mesh Observer non trovato. Assicurati che MRTK sia configurato.");
        }
    }

    void Update()
    {
        // Aggiorna il timer
        timer += Time.deltaTime;

        // Controlla se sono passati 10 secondi
        if (timer >= updateInterval)
        {
            UpdateMeshCollider();
            timer = 0.0f; // Resetta il timer
        }
    }

    private void UpdateMeshCollider()
    {
        if (meshObserver != null && meshCollider != null)
        {
            // Itera attraverso tutte le mesh osservate
            foreach (SpatialAwarenessMeshObject meshObject in meshObserver.Meshes.Values)
            {
                // Aggiorna il MeshCollider con l'ultima mesh rilevata
                meshCollider.sharedMesh = meshObject.Filter.sharedMesh;
            }
        }
    }
}
