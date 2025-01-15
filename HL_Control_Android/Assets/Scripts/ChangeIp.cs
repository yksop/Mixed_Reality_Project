using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using TMPro;

/* This class is responsible for managing the broker IP change UI and setting it in Base Client*/
public class ChangeIp : MonoBehaviour
{
    public BaseClient baseClient;
    public GameObject textInputGo;

    public void ActivateInput()
    {
        textInputGo.SetActive(true);
    }
    public void DeactivateInput()
    {
        string text = textInputGo.GetComponentInChildren<TMP_InputField>().text;
        baseClient.BCChangeIp(text);
        baseClient.restart = true;
        textInputGo.SetActive(false);
    }
}
