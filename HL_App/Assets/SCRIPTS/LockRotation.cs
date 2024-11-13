using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private void Update()
    {
        this.transform.localRotation = Quaternion.identity;
    }
}
