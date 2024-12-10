using UnityEngine;

public class PointSpawner : MonoBehaviour
{
	public RectTransform parentImage;
	public GameObject markerPrefab;

	public Vector2 position;

	public void SpawnerPoint(Vector2 position)
	{
		GameObject marker = Instantiate(markerPrefab, parentImage);

		Vector2 anchoredPosition = new Vector2(
			position.x * parentImage.rect.width - parentImage.rect.width / 2,
			position.y * parentImage.rect.height - parentImage.rect.height / 2
		);

		RectTransform markerRect = marker.GetComponent<RectTransform>();
		markerRect.anchoredPosition = anchoredPosition;
	}
}
