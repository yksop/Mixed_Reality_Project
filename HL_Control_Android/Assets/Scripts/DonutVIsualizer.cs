using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
    public void CreateDonutAtPosition(UnityEngine.Vector2 newVector2)
    {
        UnityEngine.Vector3 position = newVector2*multiplier;
        Debug.Log("Spawning a donut in " + position);
        GameObject instDonut = Instantiate(prefab, position + center.transform.position, UnityEngine.Quaternion.identity, visualizer.transform);
        donuts.Add(instDonut);
        donutCounter.UpdateCounter(0, 1);
    }

    // Destory the donut in the position pos
    public void DestroyDonutAtPosition(UnityEngine.Vector2 pos)
    {
        UnityEngine.Vector3 position = pos * multiplier;
        foreach (GameObject donut in donuts)
        {
            if ((donut.transform.position - (position + center.transform.position)).sqrMagnitude <= 0.01f)
            {
                Destroy(donut);
                donuts.Remove(donut);
                // The donut was collected, so add 1 to collected donuts
                donutCounter.UpdateCounter(1, 0);
                break;
            }
        }
    }

    // Destroy all donuts
    public void DestroyAllDonuts()
    {
        foreach (GameObject donut in donuts)
        {
            Destroy(donut);
        }
        donuts.Clear();
    }
}