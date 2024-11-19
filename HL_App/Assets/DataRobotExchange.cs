using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using M2MqttUnity;

public class DataRobotExchange : MonoBehaviour
{
    public GameObject Robottino; // Riferimento al robot
    public GameObject Center;    // Riferimento al punto centrale
    public GameObject Player;    // Riferimento al player
    public float maxDistance = 10f; // Distanza massima del raycast
    private int layerMask; // Layer mask per il raycast

    public BaseClient baseClient;

    private void Start()
    {
        // Imposta il layer mask per il layer "SpatialAwareness"
        layerMask = LayerMask.GetMask("SpatialAwareness");
    }

    private void FixedUpdate()
    {
        // Compute & Send impacto points
        CalcolaPuntiImpattoCircolari();
        
        baseClient.SendPosRot(
            Robottino,
                Robottino.transform.position - Center.transform.position,
                    Robottino.transform.rotation   
        );
        
        baseClient.SendPosRot(
            Player,
                Player.transform.position - Center.transform.position,
                    Player.transform.rotation   
        );
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
    public void CalcolaPuntiImpattoCircolari()
    {
        List<byte> puntiImpattoBytes = new List<byte>();

        // Posizione di partenza del raycast, un metro sopra il centro
        //Vector3 startPosition = Center.transform.position + new Vector3(0, 1, 0);
        Vector3 startPosition = Player.transform.position; 

        // Risoluzione angolare di 5 gradi per coprire 360 gradi
        int stepDegrees = 2;
        for (int angle = 0; angle < 360; angle += stepDegrees)
        {
            // Calcola la direzione del raycast per l'angolo attuale (sul piano X,Z)
            float rad = Mathf.Deg2Rad * angle;
            Vector3 direction = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

            // Esegue il raycast dalla posizione sopraelevata in questa direzione orizzontale
            if (Physics.Raycast(startPosition, direction, out RaycastHit hit, maxDistance, layerMask))
            {
                // Ottiene il punto di impatto
                Vector3 puntoImpatto = hit.point;
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

        // Ritorna un array di array di byte, con ogni sotto-array rappresentante le componenti X e Z di un punto di impatto
        
        baseClient.SendRoom(puntiImpattoBytes.ToArray());

    }
}

