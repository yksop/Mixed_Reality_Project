using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrieveData : MonoBehaviour
{

	private Transform playerTransform;

	public Transform robotVirtual;

	private Vector2 previousPosition;

	private float playerSpeed;

	// Start is called before the first frame update
	void Start()
	{
		playerTransform = Camera.main.transform;

		if (playerTransform != null)
		{
			previousPosition = playerTransform.position;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (robotVirtual == null || playerTransform == null) return;

		float distance = Vector2.Distance(playerTransform.position, robotVirtual.position);

		playerSpeed = Vector2.Distance(playerTransform.position, previousPosition) / Time.deltaTime;

		Debug.Log($"Posizione player: {playerTransform.position}");
		Debug.Log($"Distanza dal robot: {distance} metri");
		Debug.Log($"Velocità del player: {playerSpeed} metri al secondo");

		previousPosition = playerTransform.position;
	}
}
