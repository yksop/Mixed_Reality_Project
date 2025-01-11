using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RetrieveData : MonoBehaviour
{
    public int FATTORE_DI_SCALA = 1;
    public TextMeshProUGUI textVelocita;
    public TextMeshProUGUI textDistanza;
    public Transform playerTransform;
    public Transform robotVirtual;
    public Transform graphContainerV;
    public Transform graphContainerP;


    private Vector2 previousPosition;
    public float playerSpeed; // Velocità corrente
    public List<float> playerSpeeds = new List<float>(); // Storico velocità
    public List<float> playerDistances = new List<float>(); // Storico distanze

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

        // Salva la distanza nella lista
        //playerDistances.Add(distance);
    }

    public void StartCoroutineUpdatePlayerSpeedRoutine()
    {
        StartCoroutine(UpdatePlayerSpeedRoutine(1f));
    }

    private IEnumerator UpdatePlayerSpeedRoutine(float updateInterval = 1f)
    {
        while (true)
        {
            if (playerTransform != null && robotVirtual != null)
            {
                playerSpeed = (Vector2.Distance(playerTransform.position, previousPosition)) / FATTORE_DI_SCALA;
                textVelocita.text = $"Velocità: {playerSpeed:F2} m/s";
                previousPosition = playerTransform.position;

                // Aggiungi la velocità corrente alla lista
                playerSpeeds.Add(playerSpeed);

                // Calcola la distanza
                float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;

                playerDistances.Add(distance);
            }



            yield return new WaitForSeconds(updateInterval); // Update every second


        }
    }
    // Metodo per resettare i dati
    public void ResetGraphData(bool eraseDataLists)
    {
        if(eraseDataLists)
        {
            // Svuota le liste
            playerSpeeds.Clear();
            playerDistances.Clear();
            Debug.Log("Grafici resettati!");
        }
        // Trova ed elimina tutti i GameObject con il tag "GraphicalElement" che sono figli di graphContainerV
        foreach (Transform child in graphContainerV)
        {
            if (child.CompareTag("GraphicalElement"))
            {
                Destroy(child.gameObject); // Elimina il GameObject
            }
        }
        // Trova ed elimina tutti i GameObject con il tag "GraphicalElement" che sono figli di graphContainerP
        foreach (Transform child in graphContainerP)
        {
            if (child.CompareTag("GraphicalElement"))
            {
                Destroy(child.gameObject); // Elimina il GameObject
            }
        }

        // Ricalcola i valori di velocità e distanza basandoti sulle posizioni attuali
        if (playerTransform != null && robotVirtual != null)
        {
            // Reimposta la posizione precedente del giocatore
            previousPosition = playerTransform.position;

            // Ricalcola i valori per un breve periodo (ad esempio 5 aggiornamenti)
            for (int i = 0; i < 5; i++)
            {
                // Simula un intervallo di aggiornamento per calcolare velocità e distanza
                float distance = Vector2.Distance(playerTransform.position, robotVirtual.position) / FATTORE_DI_SCALA;
                playerDistances.Add(distance);

                float speed = (Vector2.Distance(playerTransform.position, previousPosition)) / FATTORE_DI_SCALA;
                playerSpeeds.Add(speed);

                // Aggiorna la posizione precedente
                previousPosition = playerTransform.position;
            }
        }

        // Ripristina il calcolo in tempo reale con la coroutine
        StartCoroutine(UpdatePlayerSpeedRoutine(1f));
    }

} 






