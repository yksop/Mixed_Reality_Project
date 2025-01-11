using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LavaToggle : MonoBehaviour
{
    public TMP_Text tText;
    public bool isLavaOn = false;
    public void LavaToggleText()
    {
        if(isLavaOn)
        {
            tText.text = "Lava Off";
        }
        else
        {
            tText.text = "Lava On";
        }
    }
}
