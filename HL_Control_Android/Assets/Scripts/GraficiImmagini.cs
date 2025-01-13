


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GraficiImmagini : MonoBehaviour
{
    public RectTransform graphContainer; // Contenitore per il grafico (assegnato nel Canvas)
    public GameObject pointPrefab; // Prefab per i punti (un piccolo GameObject UI come immagine)
    public Color lineColor = Color.blue; // Colore delle linee
    public float lineWidth = 2f; // Spessore delle linee

    public RetrieveData retrieveData;

    private List<GameObject> graphObjects = new List<GameObject>(); // Per gestire dinamicamente gli elementi del grafico

    public void DrawGraph()
    {
        // Rimuove eventuali oggetti grafici precedenti
        ClearGraph();

        List<float> playerSpeeds = retrieveData.playerSpeeds;
        if (playerSpeeds == null || playerSpeeds.Count < 2) return;

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        float yMax = Mathf.Max(playerSpeeds.ToArray());
        float xStep = graphWidth / (playerSpeeds.Count - 1);

        Vector2 previousPoint = Vector2.zero;

        for (int i = 0; i < playerSpeeds.Count; i++)
        {
            float xPosition = i * xStep;
            float yPosition = (playerSpeeds[i] / yMax) * graphHeight;
            Vector2 currentPoint = new Vector2(xPosition, yPosition);

            // Disegna il punto
            DrawPoint(currentPoint);

            // Disegna la linea
            if (i > 0)
            {
                DrawLine(previousPoint, currentPoint);
            }

            previousPoint = currentPoint;
        }
    }

    private void DrawPoint(Vector2 anchoredPosition)
    {
        GameObject point = Instantiate(pointPrefab, graphContainer);
        point.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        graphObjects.Add(point);
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        GameObject line = new GameObject("Line", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = lineColor;

        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 direction = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rectTransform.sizeDelta = new Vector2(distance, lineWidth);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0.5f);
        rectTransform.anchoredPosition = start;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        graphObjects.Add(line);
    }

    private void ClearGraph()
    {
        foreach (GameObject obj in graphObjects)
        {
            Destroy(obj);
        }
        graphObjects.Clear();
    }
}
