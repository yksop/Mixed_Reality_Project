using System.Collections;
using UnityEngine;

/// <summary>
/// This class is responsible for toggling the visibility of a terrain GameObject with a fade in and fade out effect.
/// It manages the activation and deactivation of the GameObject and smoothly transitions its transparency.
/// </summary>
public class TerrainToggle : MonoBehaviour
{
    // Public GameObject to be assigned from the Inspector
    public GameObject terrainObject;

    // Speed of the fade in and fade out
    public float fadeSpeed = 1f;

    // Variables to manage the state of the object
    private bool isFadingIn = false;
    private bool isActive = false;

    // Reference to the material of the object
    private Material terrainMaterial;

    void Start()
    {
        // Initialize the material and set the transparency to 0
        terrainMaterial = terrainObject.GetComponent<Renderer>().material;
        Color color = terrainMaterial.color;
        color.a = 0;
        terrainMaterial.color = color;
        
        terrainObject.SetActive(false);
        isActive = false;
    }

    // Function called by the button to toggle the object
    public void ToggleTerrainObject()
    {
        if (isActive)
        {
            // If active, start fade out and deactivate
            StartCoroutine(FadeObject(0, false));
        }
        else
        {
            // If inactive, activate and start fade in
            terrainObject.SetActive(true);
            StartCoroutine(FadeObject(1, true));
        }
    }

    // Coroutine to manage the fade in and fade out
    private IEnumerator FadeObject(float targetAlpha, bool activate)
    {
        isFadingIn = activate;
        Color color = terrainMaterial.color;
        float startAlpha = color.a;

        // Gradually change the alpha to create the fade effect
        while (Mathf.Abs(color.a - targetAlpha) > 0.01f)
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            terrainMaterial.color = color;
            yield return null; // Wait for the next frame
        }

        // Update the final state of the object
        isActive = activate;
        if (!activate)
        {
            terrainObject.SetActive(false); // Deactivate the object once the fade out is complete
        }
    }
}