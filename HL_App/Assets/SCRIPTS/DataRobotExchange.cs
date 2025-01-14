using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using M2MqttUnity;
using System.Text;

public class DataRobotExchange : MonoBehaviour
{
    public GameObject Robottino; // Riferimento al robot
    public GameObject Center;    // Riferimento al punto centrale
    public GameObject Player;    // Riferimento al player
    public float maxDistance = 10f; // Distanza massima del raycast
    private int layerMask; // Layer mask per il raycast
    public int rayPerDegree = 1;
    public float lowRayHeight = 0.3f;
    public float highRayHeight = 2f;
    public float boxWidth = 0.005f;
    public float boxHeight = 0.1f;
    private string h;
    public float bubble = 1f; // Radius of the bubble to be ignored around the player

    public BaseClient baseClient;

    private void Start()
    {
        // Imposta il layer mask per il layer "SpatialAwareness"
        layerMask = LayerMask.GetMask("SpatialAwareness");
    }

    private void FixedUpdate()
    {
        CalcolaPuntiImpattoCircolari(highRayHeight);
        CalcolaPuntiImpattoCircolari(lowRayHeight);
        baseClient.SendPosRot(Robottino, Robottino.transform.position - Center.transform.position, Robottino.transform.rotation);
        baseClient.SendPosRot(Player, Player.transform.position - Center.transform.position, Player.transform.rotation);
    }

    // Funzione che calcola la posizione del robot rispetto al centro e salva i valori in un array di byte[]
    public byte[] CalcolaPosizione()
    {
        // Ottiene la posizione relativa del robot rispetto al centro solo sul piano X,Z
        Vector3 posizioneRelativa = Robottino.transform.position - Center.transform.position;

        // Lista di byte per memorizzare solo X e Z
        List<byte> byteArray = new List<byte>();

        // Converte la coordinata X
        byteArray.AddRange(BitConverter.GetBytes(posizioneRelativa.x));

        // Converte la coordinata Z
        byteArray.AddRange(BitConverter.GetBytes(posizioneRelativa.z));

        // Ritorna l'array di byte con solo X e Z
        return byteArray.ToArray();
    }

    public byte[] CalcolaDirezioneGuardataEQuaternion()
    {
        // Direzione "forward" del robot (dove sta guardando), normalizzata sul piano X,Z
        Vector3 direzioneGuardata = new Vector3(Robottino.transform.forward.x, 0, Robottino.transform.forward.z).normalized;

        // Rotazione del robot in quaternion
        Quaternion rotazioneGuardata = Robottino.transform.rotation;

        // Lista di byte per memorizzare dati
        List<byte> byteArray = new List<byte>();

        // Converte la coordinata X della direzione guardata
        byteArray.AddRange(BitConverter.GetBytes(direzioneGuardata.x));

        // Converte la coordinata Z della direzione guardata
        byteArray.AddRange(BitConverter.GetBytes(direzioneGuardata.z));

        // Converte i componenti del quaternion (x, y, z, w)
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.x));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.y));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.z));
        byteArray.AddRange(BitConverter.GetBytes(rotazioneGuardata.w));

        // Ritorna l'array di byte con X, Z della direzione guardata e il quaternion
        return byteArray.ToArray();
    }




    // Funzione per calcolare i punti di impatto dei raycast in un array di byte[]
    //public byte[][] CalcolaPuntiImpattoCircolari()
    public void CalcolaPuntiImpattoCircolari(float height)
    {
        List<byte> puntiImpattoBytes = new List<byte>();
        float ang = 0;

        // Posizione di partenza del raycast
        Vector3 startPosition;
        //Vector3 startPosition = new Vector3(Player.transform.position.x, height, Player.transform.position.z);

        // Risoluzione angolare di 1 grado per coprire 360 gradi
        int stepDegrees = rayPerDegree;

        // Boxcast: Initial position of box (center of player) and dimensions
        Vector3 boxCenter;
        Vector3 boxHalfExtents = new Vector3(boxWidth, boxHeight, boxWidth);

        for (int angle = 0; angle < 360; angle += stepDegrees)
        {
            // Calcola la direzione del raycast per l'angolo attuale (sul piano X,Z)
            float rad = Mathf.Deg2Rad * angle;
            startPosition = new Vector3(Player.transform.position.x + bubble * Mathf.Cos(rad), height, Player.transform.position.z + bubble * Mathf.Sin(rad));
            boxCenter = startPosition;
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

            // Esegue il raycast dalla posizione sopraelevata in questa direzione orizzontale
            if (Physics.BoxCast(boxCenter, boxHalfExtents, direction, out RaycastHit hit, Quaternion.identity, maxDistance, layerMask))
            //if (Physics.Raycast(boxCenter, direction, out RaycastHit hit, maxDistance, layerMask))
            {
                // Ottiene il punto di impatto respect to vuforia marker
                Vector3 puntoImpatto = hit.point - Center.transform.position;
                /* 
                // Converte solo le componenti X e Z in un array di byte
                List<byte> puntoBytes = new List<byte>();
                puntoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.x));
                puntoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.z));
                */

                puntiImpattoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.x));
                puntiImpattoBytes.AddRange(BitConverter.GetBytes(puntoImpatto.z));

                // Aggiunge l'array di byte alla lista dei punti di impatto
                //puntiImpattoBytes.AddRange(puntoBytes.ToArray()); // 
            }
        }

        if (height == lowRayHeight)
        {
            h = "low";
        }
        else
        {
            h = "high";
        }
        baseClient.SendRoom(h, puntiImpattoBytes.ToArray());
    }

    // Function to change the height of the low boxcast height
    public void ChangeLowHeight(float num)
    {
        lowRayHeight = num;
    }

    // Function to change the height of the high boxcast height
    public void ChangeHighHeight(float num)
    {
        highRayHeight = num;
    }

    // Function to change the radius of the bubble aroud the player to ignore boxcast
    public void ChangeBubble(float num)
    {
        bubble = num;
    }
}