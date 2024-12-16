using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhisicalData : MonoBehaviour
{
    public GameObject player;
    public GameObject avatar;
    private Vector3 previousPosition;
    private Vector3 velocity;
    public TextMeshProUGUI textVelocity;
    public TextMeshProUGUI textDistance;
     private int SCALE_DISTANCE_FACTOR = 30;

    void Start()
    {
        previousPosition = player.transform.position; // Salva la posizione iniziale
    }

    void Update()
    {
        // Calcola la velocità come variazione di posizione
        velocity = (player.transform.position - previousPosition) / Time.deltaTime;

        // Aggiorna la posizione precedente
        previousPosition = player.transform.position;

        //Debug.Log($"Velocità: {velocity}, Modulo: {velocity.magnitude}");

        textVelocity.text = $"Velocity:\n {velocity.magnitude:F1} [m/s]";
        textDistance.text = $"Robot Distance:\n {(player.transform.position - avatar.transform.position).magnitude/SCALE_DISTANCE_FACTOR:F1} [m]";

    }

}
