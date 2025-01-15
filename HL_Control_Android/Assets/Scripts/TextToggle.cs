using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* This class toggles a flag and changes the text on the button to toggle it (label + "on"/"off") when it is pressed*/
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
        isOn = !isOn;
    }
}
