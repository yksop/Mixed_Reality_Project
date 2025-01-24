using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutVisualizer : MonoBehaviour
{

    public GameObject prefab;

    public void CreateDonutAtPosition(Vector2 newVector2)
    {
        // Usa i valori di newVector2 per creare un Vector3
        Vector3 position = newVector2;
        Instantiate(prefab, position, Quaternion.identity);
    }
}
