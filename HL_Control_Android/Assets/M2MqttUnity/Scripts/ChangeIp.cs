using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using TMPro;

public class ChangeIp : MonoBehaviour
{
    public BaseClient baseClient;
    public GameObject textInputGo;
    public GameObject raycastInputGo;

    public void ActivateInput()
    {
        textInputGo.SetActive(true);
    }

    public void ActivateRaycastInput()
    {
        raycastInputGo.SetActive(true);
    }

    public void DeactivateInput()
    {
        string text = textInputGo.GetComponentInChildren<TMP_InputField>().text;
        baseClient.BCChangeIp(text);
        baseClient.restart = true;
        textInputGo.SetActive(false);
    }
    public void DeactivateRaycastInput()
    {
        textInputGo.SetActive(false);
    }
}
