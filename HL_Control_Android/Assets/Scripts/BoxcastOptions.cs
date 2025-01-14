using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using TMPro;

public class BoxcastOptions : MonoBehaviour
{
    public BaseClient baseClient;
    public GameObject textInputGo;
    public TMP_InputField lowField;
    public TMP_InputField highField;
    public TMP_InputField bubbleField;

    public void ActivateInput()
    {
        textInputGo.SetActive(true);
    }
    public void DeactivateInput()
    {
        Debug.Log("Pressed");
        if (lowField.text != "")
        {
            // Change the low boxcast height
            baseClient.BCChangeLow(float.Parse(lowField.text));
        }
        if (highField.text != "")
        {
            //Change the high boxcast height
            baseClient.BCChangeHigh(float.Parse(highField.text));
        }
        if (bubbleField.text != "")
        {
            // Change the ignore boxcast bubble radius
            baseClient.BCChangeBubble(float.Parse(bubbleField.text));
        }
        textInputGo.SetActive(false);
    }
}
