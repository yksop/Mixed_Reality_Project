using System.Collections;
using UnityEngine;
using TMPro;

public class RetrieveData : MonoBehaviour
{
	public int FATTORE_DI_SCALA = 1;
	public TextMeshProUGUI textVelocita;
	public TextMeshProUGUI textDistanza;
	public Transform playerTransform;
	public Transform robotVirtual;

	private Vector2 previousPosition;
	private float playerSpeed;

	// Start is called before the first frame update
	void Start()
	{
		if (playerTransform != null)
		{
			previousPosition = playerTransform.position;
		}

		StartCoroutine(UpdatePlayerSpeedRoutine(1f));
	}

	// Update is called once per frame
	void Update()
	{
		if (robotVirtual == null || playerTransform == null) return;

		float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;
		textDistanza.text = $"Distanza: {distance:F2} m";
	}

	private IEnumerator UpdatePlayerSpeedRoutine(float updateInterval = 1f)
	{
		while (true)
		{
			if (playerTransform != null)
			{
				playerSpeed = (Vector2.Distance(playerTransform.position, previousPosition)/*  / Time.deltaTime */) / FATTORE_DI_SCALA;
				textVelocita.text = $"Velocità: {playerSpeed:F2} m/s";
				previousPosition = playerTransform.position;
			}

			yield return new WaitForSeconds(updateInterval); // Update every second
		}
	}
}