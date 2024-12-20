using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextToggle : MonoBehaviour
{
    public string label;
    public TMP_Text tText;
    public bool isOn = false;
    public void ToggleText()
    {
        if (isOn)
        {
            tText.text = label + " Off";
        }
        else
        {
            tText.text = label + " On";
        }
    }
}
