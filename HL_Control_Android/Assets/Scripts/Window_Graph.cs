
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Window_Graph : MonoBehaviour
{
    public RetrieveData retrieveData; // Riferimento allo script RetrieveData
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private float Width;

    public void ShowGraphonButtonPress()
    {
        // Recupera il componente RectTransform del contenitore del grafico
        graphContainer = transform.Find("graphContainer V").GetComponent<RectTransform>();
        Width = graphContainer.rect.width;

        // Verifica che RetrieveData sia collegato e contenga dati
        if (retrieveData != null && retrieveData.playerSpeeds.Count > 0)
        {
            // Crea il grafico usando la lista playerSpeeds
            ShowGraph(retrieveData.playerSpeeds);
        }
        else
        {
            Debug.LogWarning("RetrieveData non è collegato o non ci sono dati in playerSpeeds.");
        }

        // Calcola medie
        float avgSpeed = retrieveData.playerSpeeds.Average();
        float avgDistance = retrieveData.playerDistances.Average();

        // Mostra riquadro sopra il grafico
        CreateInfoBox($"Velocità Media: {avgSpeed:F2} m/s");



    }


    private void CreateInfoBox(string content)
    {
        // Crea un riquadro
        GameObject infoBox = new GameObject("InfoBox", typeof(Image));
        infoBox.transform.SetParent(graphContainer, false);
        Image infoBoxImage = infoBox.GetComponent<Image>();
        infoBoxImage.color = new Color(0, 0, 0, 0.5f); // Sfondo semitrasparente

        RectTransform rectTransform = infoBox.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(650, 120); // Dimensioni del riquadro
        rectTransform.anchoredPosition = new Vector2(1099, 410); // Posiziona riquadro infoBox
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        // Crea testo dentro il riquadro
        GameObject textObject = new GameObject("InfoText", typeof(Text));
        textObject.transform.SetParent(infoBox.transform, false);
        Text text = textObject.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 50;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform textRectTransform = text.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = rectTransform.sizeDelta; // Match dimensioni del riquadro
        textRectTransform.anchoredPosition = Vector2.zero;
        infoBox.tag = "GraphicalElement"; // Assegna il tag GraphicalElement
    }



    private void CreateCircle(Vector2 anchoredPosition)
    {
        // Crea un elemento grafico a forma di cerchio per rappresentare un punto del grafico
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        gameObject.layer = LayerMask.NameToLayer("Graph");
        gameObject.tag = "GraphicalElement"; // Assegna il tag GraphicalElement
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    private void CreateLine(Vector2 startPosition, Vector2 endPosition)
    {   
        
        // Crea una linea tra due punti del grafico
        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = Color.white;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 direction = (endPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, endPosition);

        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchoredPosition = startPosition + direction * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        gameObject.layer = LayerMask.NameToLayer("Graph");
        gameObject.tag = "GraphicalElement"; // Assegna il tag GraphicalElement
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    private void CreateLabel(string text, Vector2 position, Transform parent)
    {
        GameObject labelObject = new GameObject("Label", typeof(Text));
        labelObject.transform.SetParent(parent, false);
        Text labelText = labelObject.GetComponent<Text>();
        labelText.text = text;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 15;
        labelText.color = Color.white;
        RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(100, 20);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        labelObject.tag = "GraphicalElement"; // Assegna il tag GraphicalElement
    }

    private void ShowGraph(List<float> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y; // Altezza del grafico
        float yMaximum = 15f; // Valore massimo di riferimento per il grafico (adattalo se necessario)
        float xSize = Width/valueList.Count; // Distanza tra i punti lungo l'asse X

        // Crea il primo punto del grafico
        Vector2 previousPosition = Vector2.zero;

        for (int i = 0; i < valueList.Count; i++)
        {
            // Calcola la posizione del punto corrente
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            Vector2 currentPosition = new Vector2(xPosition, yPosition);

            // Crea il cerchio per il punto corrente
            CreateCircle(currentPosition);

            // Disegna una linea tra i punti (opzionale)
            if (i > 0)
            {
                CreateLine(previousPosition, currentPosition);
            }

            previousPosition = currentPosition;
        }


        // Etichette sull'asse X
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = (i + 1) * xSize; // Sposta i valori di uno verso destra
            CreateLabel(i.ToString(), new Vector2(xPosition, -20), graphContainer);
        }

        // Etichetta sull'asse Y
        GameObject labelY = new GameObject("YLabel", typeof(Text));
        labelY.transform.SetParent(graphContainer, false);
        Text labelYText = labelY.GetComponent<Text>();
        labelYText.text = "Velocità (m/s)";
        labelYText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelYText.fontSize = 40;
        labelYText.color = Color.white;

        // Posiziona e ruota la scritta
        RectTransform labelYRect = labelY.GetComponent<RectTransform>();
        labelYRect.anchoredPosition = new Vector2(20, graphContainer.sizeDelta.y / 2);
        labelYRect.sizeDelta = new Vector2(250, 150);
        labelYRect.anchorMin = new Vector2(0, 0);
        labelYRect.anchorMax = new Vector2(0, 0);
        labelYRect.localEulerAngles = new Vector3(0, 0, 90); // Ruota di 90° in senso antiorario









    }







}