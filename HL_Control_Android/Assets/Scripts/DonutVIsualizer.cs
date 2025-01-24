using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class updates the visualization of the donuts/rewards transmitted from the Hololens through the baseClient
/// </summary>
public class DonutVisualizer : MonoBehaviour
{

    public GameObject prefab;
    public List<GameObject> donuts;
    public float multiplier = 100f;

    // Generate donuts in the requested position newVector2
    public void CreateDonutAtPosition(Vector2 newVector2)
    {
        Vector3 position = newVector2*multiplier;
        Instantiate(prefab, position, Quaternion.identity);
        donuts.Add(prefab);
    }

    // Destory the donuts in the position pos
    public void DestroyDonutAtPosition(Vector2 pos)
    {
        foreach (GameObject donut in donuts)
        {
            Vector3 position = pos*multiplier;
            if (donut.transform.position == position)
            {
                Destroy(donut);
            }
        }
    }
}