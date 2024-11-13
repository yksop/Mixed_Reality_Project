using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycasting : MonoBehaviour
{
    public Camera cam;
    public GameObject cub;
    LayerMask layerMask = 1 << 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray lastRay = cam.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(lastRay.origin, lastRay.direction * 20, Color.green, 1);

            if (Physics.Raycast(lastRay, out hit, 20, layerMask))
            {
                Debug.Log("Intersect object:" + hit.collider.gameObject);
                Debug.Log("distance:" + hit.distance);
                cub.GetComponent<Renderer>().material.color = Color.red;

            }
        }
 
    }
}
