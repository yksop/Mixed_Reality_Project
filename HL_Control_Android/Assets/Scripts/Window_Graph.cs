using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// The Window_Graph class is responsible for creating and displaying a graphical representation
/// of data within a Unity UI. It retrieves data from a linked RetrieveData script, and uses this
/// data to plot points, draw lines between points, and display labels on a graph. The class also
/// includes functionality to display an information box with average values above the graph.
/// </summary>
public class Window_Graph : MonoBehaviour
{
    public RetrieveData retrieveData; // Reference to the RetrieveData script
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private float Width;

    public void ShowGraphonButtonPress()
    {
        // Retrieve the RectTransform component of the graph container
        graphContainer = transform.Find("graphContainer V").GetComponent<RectTransform>();
        Width = graphContainer.rect.width;

        // Check if RetrieveData is linked and contains data
        if (retrieveData != null && retrieveData.playerSpeeds.Count > 0)
        {
            // Create the graph using the playerSpeeds list
            ShowGraph(retrieveData.playerSpeeds);
        }
        else
        {
            Debug.LogWarning("RetrieveData is not linked or there is no data in playerSpeeds.");
        }

        // Calculate averages
        float avgSpeed = retrieveData.playerSpeeds.Average();
        float avgDistance = retrieveData.playerDistances.Average();

        // Show info box above the graph
        CreateInfoBox($"Average Speed: {avgSpeed:F2} m/s");
    }

    private void CreateInfoBox(string content)
    {
        // Create an info box
        GameObject infoBox = new GameObject("InfoBox", typeof(Image));
        infoBox.transform.SetParent(graphContainer, false);
        Image infoBoxImage = infoBox.GetComponent<Image>();
        infoBoxImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent background

        RectTransform rectTransform = infoBox.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(650, 120); // Box dimensions
        rectTransform.anchoredPosition = new Vector2(1099, 410); // Position the info box
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        // Create text inside the box
        GameObject textObject = new GameObject("InfoText", typeof(Text));
        textObject.transform.SetParent(infoBox.transform, false);
        Text text = textObject.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 50;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform textRectTransform = text.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = rectTransform.sizeDelta; // Match box dimensions
        textRectTransform.anchoredPosition = Vector2.zero;
        infoBox.tag = "GraphicalElement"; // Assign the tag GraphicalElement
    }

    private void CreateCircle(Vector2 anchoredPosition)
    {
        // Create a circle graphic element to represent a point on the graph
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        gameObject.layer = LayerMask.NameToLayer("Graph");
        gameObject.tag = "GraphicalElement"; // Assign the tag GraphicalElement
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    private void CreateLine(Vector2 startPosition, Vector2 endPosition)
    {
        // Create a line between two points on the graph
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
        gameObject.tag = "GraphicalElement"; // Assign the tag GraphicalElement
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
        labelObject.tag = "GraphicalElement"; // Assign the tag GraphicalElement
    }

    private void ShowGraph(List<float> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y; // Graph height
        float yMaximum = 15f; // Maximum reference value for the graph (adjust if necessary)
        float xSize = Width / valueList.Count; // Distance between points along the X axis

        // Create the first point of the graph
        Vector2 previousPosition = Vector2.zero;

        for (int i = 0; i < valueList.Count; i++)
        {
            // Calculate the position of the current point
            float xPosition = i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            Vector2 currentPosition = new Vector2(xPosition, yPosition);

            // Create the circle for the current point
            CreateCircle(currentPosition);

            // Draw a line between the points
            if (i > 0)
            {
                CreateLine(previousPosition, currentPosition);
            }

            previousPosition = currentPosition;
        }

        // Labels on the X axis
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = (i + 1) * xSize; // Shift values one to the right
            CreateLabel(i.ToString(), new Vector2(xPosition, -20), graphContainer);
        }

        // Label on the Y axis
        GameObject labelY = new GameObject("YLabel", typeof(Text));
        labelY.transform.SetParent(graphContainer, false);
        Text labelYText = labelY.GetComponent<Text>();
        labelYText.text = "Speed (m/s)";
        labelYText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelYText.fontSize = 40;
        labelYText.color = Color.white;

        // Position and rotate the text
        RectTransform labelYRect = labelY.GetComponent<RectTransform>();
        labelYRect.anchoredPosition = new Vector2(20, graphContainer.sizeDelta.y / 2);
        labelYRect.sizeDelta = new Vector2(250, 150);
        labelYRect.anchorMin = new Vector2(0, 0);
        labelYRect.anchorMax = new Vector2(0, 0);
        labelYRect.localEulerAngles = new Vector3(0, 0, 90); // Rotate 90° counterclockwise
    }
}