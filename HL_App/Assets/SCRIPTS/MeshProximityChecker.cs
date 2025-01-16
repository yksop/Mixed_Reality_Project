/* using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

public class MeshProximityChecker : MonoBehaviour
{
    public GameObject targetObject; // Assegna qui il GameObject da monitorare
    public float detectionDistance = 1.0f; // Distanza di rilevamento sul piano XZ

    private List<MeshCollider> meshColliders = new List<MeshCollider>();
    private RobotController robotController;

    void Start()
    {
        // Recupera il primo osservatore di mesh disponibile
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();

        if (observer != null)
        {
            // Itera su tutte le mesh rilevate
            foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
            {
                // Aggiungi un collider alla mesh per rilevare le collisioni
                MeshCollider meshCollider = meshObject.Filter.gameObject.GetComponent<MeshCollider>();
                if (meshCollider == null)
                {
                    meshCollider = meshObject.Filter.gameObject.AddComponent<MeshCollider>();
                }

                meshColliders.Add(meshCollider);
            }
        }
        else
        {
            Debug.LogWarning("Nessun osservatore di mesh disponibile.");
        }

        // Recupera il componente RobotController da targetObject
        if (targetObject != null)
        {
            robotController = targetObject.GetComponent<RobotController>();
            if (robotController == null)
            {
                Debug.LogWarning("RobotController non trovato in targetObject.");
            }
        }
    }

    void Update()
    {
        if (targetObject != null && robotController != null)
        {
            foreach (MeshCollider meshCollider in meshColliders)
            {
                // Trova il punto più vicino della mesh rispetto all'oggetto target
                Vector3 closestPoint = meshCollider.ClosestPoint(targetObject.transform.position);

                // Ignora l'asse Y, consideriamo solo X e Z per il piano orizzontale
                Vector3 targetPositionXZ = new Vector3(targetObject.transform.position.x, 0, targetObject.transform.position.z);
                Vector3 closestPointXZ = new Vector3(closestPoint.x, 0, closestPoint.z);

                // Calcola la distanza sul piano XZ
                float distanceXZ = Vector3.Distance(targetPositionXZ, closestPointXZ);

                // Se il GameObject è vicino alla mesh, esegui un'azione
                if (distanceXZ <= detectionDistance)
                {
                    Debug.Log("Il target si trova vicino a una mesh rilevata sul piano XZ!");
                    robotController.SetMovingFalse(); // Usa l'istanza per chiamare il metodo
                    break;
                }
            }
        }
    }
}
 */