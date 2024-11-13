using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScale : MonoBehaviour
{
    private void Update()
    {
        this.transform.localScale = new Vector3(0.40f,0.40f,0.80f);
    }
}