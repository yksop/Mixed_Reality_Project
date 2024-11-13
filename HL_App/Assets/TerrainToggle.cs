using System.Collections;
using UnityEngine;

public class TerrainToggle : MonoBehaviour
{
    // GameObject pubblico da assegnare dall'Inspector
    public GameObject terrainObject;

    // Velocità del fade in e fade out
    public float fadeSpeed = 1f;

    // Variabili per gestire lo stato dell'oggetto
    private bool isFadingIn = false;
    private bool isActive = false;

    // Riferimento al materiale dell'oggetto
    private Material terrainMaterial;

    void Start()
    {
        // Inizializza il materiale e setta la trasparenza a 0
        terrainMaterial = terrainObject.GetComponent<Renderer>().material;
        Color color = terrainMaterial.color;
        color.a = 0;
        terrainMaterial.color = color;
        
        terrainObject.SetActive(false);
        isActive = false;
    }

    // Funzione chiamata dal bottone per attivare/disattivare l'oggetto
    public void ToggleTerrainObject()
    {
        if (isActive)
        {
            // Se è attivo, avvia fade out e disattiva
            StartCoroutine(FadeObject(0, false));
        }
        else
        {
            // Se è inattivo, attiva e avvia fade in
            terrainObject.SetActive(true);
            StartCoroutine(FadeObject(1, true));
        }
    }

    // Coroutine per gestire il fade in e fade out
    private IEnumerator FadeObject(float targetAlpha, bool activate)
    {
        isFadingIn = activate;
        Color color = terrainMaterial.color;
        float startAlpha = color.a;

        // Cambia gradualmente l'alpha per creare l'effetto di dissolvenza
        while (Mathf.Abs(color.a - targetAlpha) > 0.01f)
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            terrainMaterial.color = color;
            yield return null; // Aspetta il frame successivo
        }

        // Aggiorna lo stato finale dell'oggetto
        isActive = activate;
        if (!activate)
        {
            terrainObject.SetActive(false); // Disattiva l'oggetto una volta completato il fade out
        }
    }
}
