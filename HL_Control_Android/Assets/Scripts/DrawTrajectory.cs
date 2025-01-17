using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using M2MqttUnity;

/// <summary>
/// The DrawTrajectory class is responsible for capturing and drawing a trajectory on a canvas
/// based on user input (mouse or touch). It also converts the drawn points to a relative coordinate
/// system centered on a target image and sends the trajectory data to BaseClient that forwards it to the Holo Lens.
/// </summary>
public class DrawTrajectory : MonoBehaviour
{
    //public GameObject centerGO;
    // Prefab di un punto da disegnare sul canvas
    [SerializeField] private GameObject pointPrefab;

    // Canvas principale
    [SerializeField] private Canvas canvas;

    // Oggetto target che deve essere toccato (un'immagine nel canvas)
    [SerializeField] private RectTransform targetImage;

    // Scaling the trajectory to match the representation of the room
    public float scalingFactor = 0.01f;

    // Lista per memorizzare i punti
    private List<GameObject> instantiatedPoints = new List<GameObject>();
    private List<Vector2> relativePoints = new List<Vector2>();
    private List<Vector2> points = new List<Vector2>();

    // Distanza minima tra i punti
    [SerializeField] private float minPointDistance = 0.5f;

    // Variabile per controllare se stiamo disegnando
    private bool isDrawing = false;

    // BaseClient Class
    public BaseClient baseClient;

    void Update()
    {
        // Gestione per mouse e touchscreen
        if (Input.GetMouseButtonDown(0) && IsPointerOverTarget(Input.mousePosition))
        {
            StartDrawing();
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            StopDrawing();
        }

        if (Input.GetMouseButton(0) && isDrawing && IsPointerOverTarget(Input.mousePosition))
        {
            Draw(Input.mousePosition);
        }
        

        // Gestione touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && IsPointerOverTarget(touch.position))
            {
                StartDrawing();
            }

            if (touch.phase == TouchPhase.Ended && isDrawing)
            {
                StopDrawing();
            }

            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDrawing && IsPointerOverTarget(touch.position))
            {
                Draw(touch.position);
            }            
        }
    }

    private void StartDrawing()
    {
        // Resetta i punti e distrugge i precedenti
        DestroyAllPoints();
        points.Clear();
        instantiatedPoints.Clear();
        isDrawing = true;
    }

    private void StopDrawing()
    {
        isDrawing = false;

        // Calcola il centro dell'immagine target
        Vector2 targetCenter = GetTargetImageCenter();

        // Converte i punti rispetto al sistema di riferimento del centro dell'immagine
        foreach (var point in points)
        {
            relativePoints.Add(new Vector2((point - targetCenter).x*scalingFactor, (point - targetCenter).y*scalingFactor));
        }

        //Debug.Log("Punti relativi salvati rispetto al centro dell'immagine: " + string.Join(", ", relativePoints));
    }

    private void Draw(Vector2 inputPosition)
    {
        // Ottieni la posizione nello spazio del canvas
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), inputPosition, canvas.worldCamera, out localPosition);

        // Aggiungi il punto se è abbastanza lontano dall'ultimo
        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], localPosition) >= minPointDistance)
        {
            points.Add(localPosition);

            // Crea un nuovo punto sul canvas
            GameObject newPoint = Instantiate(pointPrefab, canvas.transform);
            RectTransform pointTransform = newPoint.GetComponent<RectTransform>();
            pointTransform.anchoredPosition = localPosition;

            // Aggiungi il punto alla lista dei punti istanziati
            instantiatedPoints.Add(newPoint);
        }
    }

    private void DestroyAllPoints()
    {
        // Distrugge tutti i punti in modo efficiente
        //Debug.Log("Destroying " + instantiatedPoints.Count + " points!");
        for (int i = 0; i < instantiatedPoints.Count; i++)
        {
            if (instantiatedPoints[i] != null)
            {
                Destroy(instantiatedPoints[i]);
            }
        }

        // Svuota la lista
        instantiatedPoints.Clear();
        points.Clear();
        relativePoints.Clear();
    }

    // Controlla se l'input è sopra l'immagine target
    private bool IsPointerOverTarget(Vector2 inputPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetImage, inputPosition, canvas.worldCamera);
    }

    // Calcola il centro dell'immagine target nello spazio del canvas
    private Vector2 GetTargetImageCenter()
    {
        //Vector2 localCenter = new Vector2(centerGO.transform.position.x, centerGO.transform.position.y);
        Vector3[] worldCorners = new Vector3[4];
        targetImage.GetWorldCorners(worldCorners);

        // Media dei quattro angoli per ottenere il centro
        Vector3 center = (worldCorners[0] + worldCorners[2]) / 2f;

        // Converte il centro nello spazio locale del canvas
        Vector2 localCenter;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, center), canvas.worldCamera, out localCenter);

        return localCenter;
    }

    public void SendPoints()
    {
        if (relativePoints == null || relativePoints.Count == 0)
        {
            Debug.LogError("La lista dei punti relativi è vuota o null!");
            return;
        }
        //Debug.Log("Points Sent!");
        Vector2[] pointsArray = relativePoints.ToArray();
        Debug.Log(pointsArray[0].ToString());
        baseClient.SendTrajectory(pointsArray);
    }
}
