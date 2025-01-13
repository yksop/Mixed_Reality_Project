/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphSpeed : MonoBehaviour
{
  // Dati da plottare
  public List<Vector2> dataPoints = new List<Vector2>();
  public float pointSize = 0.1f; // Dimensione dei punti
  public float lineWidth = 0.02f; // Spessore delle linee
  public Color pointColor = Color.red; // Colore dei punti
  public Color lineColor = Color.blue; // Colore delle linee
  public RetrieveData retrieveData;
public int gridLines = 5;
public Color axisColor = Color.green;

public float graphWidth = 10f;
public float graphHeight = 5f;

 //  public void DrawGraph()
 // {

 //   for (int i = 0; i < retrieveData.playerSpeed.Count; i++)
 //   {
 //     dataPoints.Add(new Vector2(i, retrieveData.playerSpeed[i]));
   // }

    // Disegna il grafico
  //  AdaptGraphToData();
  //  DrawPlot();
  //}





 public void DrawGraph()
 { 
    // Assumi che retrieveData.playerSpeeds contenga i dati
    for (int i = 0; i < retrieveData.playerSpeeds.Count; i++)
    {
        dataPoints.Add(new Vector2(i, retrieveData.playerSpeeds[i])); // Usa i dati della lista
    }

    // Disegna il grafico
    AdaptGraphToData();
    DrawPlot();
 }











  void DrawPlot()
  {
    // Disegna i punti
    foreach (Vector2 point in dataPoints)
    {
      DrawPoint(point, pointColor, pointSize);
    }

    // Disegna le linee
    for (int i = 0; i < dataPoints.Count - 1; i++)
    {
      DrawLine(dataPoints[i], dataPoints[i + 1], lineColor, lineWidth);
    }

    DrawAxes();
  }

  void DrawPoint(Vector2 position, Color color, float size)
  {
    GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    point.transform.position = new Vector3(position.x, position.y, 0);
    point.transform.localScale = new Vector3(size, size, size);
    var renderer = point.GetComponent<Renderer>();
    renderer.material.color = color;
  }

  void DrawLine(Vector2 start, Vector2 end, Color color, float width)
  {
    GameObject line = new GameObject("Line");
    LineRenderer lr = line.AddComponent<LineRenderer>();
    lr.startWidth = width;
    lr.endWidth = width;
    lr.positionCount = 2;
    lr.SetPosition(0, new Vector3(start.x, start.y, 0));
    lr.SetPosition(1, new Vector3(end.x, end.y, 0));
    lr.material = new Material(Shader.Find("Sprites/Default")); // Shader per il colore
    lr.startColor = color;
    lr.endColor = color;
  }

  void DrawAxes()
  {
    // Linea X (asse orizzontale)
    DrawLine(new Vector2(-graphWidth / 2, 0), new Vector2(graphWidth / 2, 0), axisColor, 0.05f);

    // Linea Y (asse verticale)
    DrawLine(new Vector2(0, -graphHeight / 2), new Vector2(0, graphHeight / 2), axisColor, 0.05f);

    // Linee di griglia
    for (int i = 1; i <= gridLines; i++)
    {
      // Linee orizzontali
      float yPos = (graphHeight / gridLines) * i - graphHeight / 2;
      DrawLine(new Vector2(-graphWidth / 2, yPos), new Vector2(graphWidth / 2, yPos), axisColor * 0.5f, 0.01f);

      // Linee verticali
      float xPos = (graphWidth / gridLines) * i - graphWidth / 2;
      DrawLine(new Vector2(xPos, -graphHeight / 2), new Vector2(xPos, graphHeight / 2), axisColor * 0.5f, 0.01f);
    }
  }

  void AdaptGraphToData()
  {
    // Trova i valori minimi e massimi nei dati
    float minX = Mathf.Min(dataPoints[0].x, dataPoints[dataPoints.Count - 1].x);
    float maxX = Mathf.Max(dataPoints[0].x, dataPoints[dataPoints.Count - 1].x);
    float minY = Mathf.Min(dataPoints[0].y, dataPoints[dataPoints.Count - 1].y);
    float maxY = Mathf.Max(dataPoints[0].y, dataPoints[dataPoints.Count - 1].y);

    // Estendi leggermente il range per dare margine al grafico
    float paddingX = (maxX - minX) * 0.1f;
    float paddingY = (maxY - minY) * 0.1f;

    // Imposta larghezza e altezza del grafico in base ai dati
    graphWidth = (maxX - minX) + paddingX * 2;
    graphHeight = (maxY - minY) + paddingY * 2;

    // Debug per verificare i nuovi valori
    Debug.Log($"Adattato grafico: Width={graphWidth}, Height={graphHeight}, XRange=({minX}, {maxX}), YRange=({minY}, {maxY})");
  }

  Vector2 ScaleToGraph(Vector2 point, float minX, float maxX, float minY, float maxY)
  {
    float x = (point.x - minX) / (maxX - minX) * graphWidth - graphWidth / 2;
    float y = (point.y - minY) / (maxY - minY) * graphHeight - graphHeight / 2;
    return new Vector2(x, y);
  }
}

 */






using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GraphSpeed : MonoBehaviour
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
