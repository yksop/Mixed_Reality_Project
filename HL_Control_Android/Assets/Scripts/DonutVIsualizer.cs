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
    public GameObject center;
    private GameObject visualizer;
    public DonutCounter donutCounter;

    public void Start()
    {
        visualizer = this.gameObject;
    }

    // Generate donuts in the requested position newVector2
    public void CreateDonutAtPosition(Vector2 newVector2)
    {
        Vector3 position = newVector2*multiplier;
        Debug.Log("Spawning a donut in " + position);
        GameObject instDonut = Instantiate(prefab, position + center.transform.position, Quaternion.identity, visualizer.transform);
        donuts.Add(instDonut);
        donutCounter.UpdateCounter(0, 1);
    }

    // Destory the donut in the position pos
    public void DestroyDonutAtPosition(Vector2 pos)
    {
        Vector3 position = pos * multiplier;
        foreach (GameObject donut in donuts)
        {
            if (donut.transform.position == position + center.transform.position)
            {
                Destroy(donut);
                donuts.Remove(donut);
                donutCounter.UpdateCounter(1, 0);
                break;
            }
        }
    }
}