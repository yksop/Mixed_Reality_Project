using UnityEngine;

public class PointSpawner : MonoBehaviour
{
    public RectTransform parentImage; // The container for the markers
    public GameObject markerPrefab; // The prefab for individual markers

    private GameObject[] markers; // Array to store the marker GameObjects
    private const int markerCount = 180; // Total number of markers to spawn

    private void Start()
    {
        // Initialize the array of markers
        markers = new GameObject[markerCount];

        // Instantiate markers and add them as children of the parentImage
        for (int i = 0; i < markerCount; i++)
        {
            GameObject marker = Instantiate(markerPrefab, parentImage);
            marker.SetActive(false); // Markers are initially invisible
            markers[i] = marker;
        }
    }

    // Updates the markers based on an array of bytes
    public void UpdateMarkers(byte[] beats)
    {
        // Each float requires 4 bytes, so the total number of floats is beats.Length / 4
        if (beats.Length % 4 != 0)
        {
            Debug.LogError("The byte array length must be a multiple of 4 (each float is 4 bytes).\n");
            return;
        }

        // Convert the byte array into a float array
        int floatCount = beats.Length / 4;
        float[] floatArray = new float[floatCount];
        for (int i = 0; i < floatCount; i++)
        {
            floatArray[i] = System.BitConverter.ToSingle(beats, i * 4);
        }

        // Ensure the float array can form pairs of x and z
        if (floatArray.Length % 2 != 0)
        {
            Debug.LogError("The float array must contain pairs of x and z values.\n");
            return;
        }

        // Determine the number of markers to update, limited by markerCount
        int pairCount = Mathf.Min(floatArray.Length / 2, markerCount);

        for (int i = 0; i < pairCount; i++)
        {
            // Extract x and z values from the float array
            float x = floatArray[i * 2]; // First value of the pair
            float z = floatArray[i * 2 + 1]; // Second value of the pair

            // Create a position vector using the x and z values
            Vector2 position = new Vector2(x, z);

            // Update the marker's position and make it visible
            SetMarkerPosition(markers[i], position);
            markers[i].SetActive(true);
        }

        // Deactivate unused markers
        for (int i = pairCount; i < markerCount; i++)
        {
            markers[i].SetActive(false);
        }
    }

    // Sets the position of a marker based on a Vector2
    private void SetMarkerPosition(GameObject marker, Vector2 position)
    {
        // Calculate the anchored position relative to the parentImage's size
        Vector2 anchoredPosition = new Vector2(
            position.x * parentImage.rect.width - parentImage.rect.width / 2,
            position.y * parentImage.rect.height - parentImage.rect.height / 2
        );

        // Apply the anchored position to the marker's RectTransform
        RectTransform markerRect = marker.GetComponent<RectTransform>();
        markerRect.anchoredPosition = anchoredPosition;
    }
}
