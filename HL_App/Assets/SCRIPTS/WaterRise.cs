using System.Collections;
using UnityEngine;

public class WaterRise : MonoBehaviour
{
    // GameObject pubblico da assegnare dall'Inspector
    public GameObject waterObject;

    // Altezze target per il movimento dell'oggetto
    private float targetYUp = 0f;
    private float targetYDown = -1.0f;
    
    // Variabili per gestire lo stato dell'oggetto
    private bool isRising = false;
    private bool isActive = false;
    
    // Velocità di movimento
    public float moveSpeed = 1f;
    void start()
    {
        waterObject.SetActive(false);
        isActive = false;
    }

    // Funzione chiamata dal bottone per attivare/disattivare e muovere l'oggetto
    public void ToggleWaterObject()
    {
        if (isActive)
        {
            // Se è attivo, scende e si disattiva
            StartCoroutine(MoveWaterObject(targetYDown, false));
        }
        else
        {
            // Se è inattivo, sale e si attiva
            waterObject.SetActive(true);
            StartCoroutine(MoveWaterObject(targetYUp, true));
        }
    }

    // Coroutine per muovere l'oggetto
    private IEnumerator MoveWaterObject(float targetY, bool activate)
    {
        isRising = activate;
        Vector3 startPosition = waterObject.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);

        // Movimento graduale
        while (Vector3.Distance(waterObject.transform.position, targetPosition) > 0.01f)
        {
            waterObject.transform.position = Vector3.MoveTowards(waterObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Aspetta il frame successivo
        }

        // Aggiorna lo stato finale dell'oggetto
        isActive = activate;
        if (!activate)
        {
            waterObject.SetActive(false); // Disattiva l'oggetto una volta sceso
        }
    }
}
