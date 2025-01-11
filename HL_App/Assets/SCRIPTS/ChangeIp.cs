using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using TMPro;

public class ChangeIp : MonoBehaviour
{
    public BaseClient baseClient;
    public GameObject textInputGo;
    public TMP_InputField textField;
    private string text;

    // Function that opens IP interface
    public void ActivateInput()
    {
        textInputGo.SetActive(true);
    }

    // Functions called by the keyboard to write
    public void One()
    {
        text = textField.text;
        textField.text = text + "1";
    }
    public void Two()
    {
        text = textField.text;
        textField.text = text + "2";
    }
    public void Three()
    {
        text = textField.text;
        textField.text = text + "3";
    }
    public void Four()
    {
        text = textField.text;
        textField.text = text + "4";
    }
    public void Five()
    {
        text = textField.text;
        textField.text = text + "5";
    }
    public void Six()
    {
        text = textField.text;
        textField.text = text + "6";
    }
    public void Seven()
    {
        text = textField.text;
        textField.text = text + "7";
    }
    public void Eight()
    {
        text = textField.text;
        textField.text = text + "8";
    }
    public void Nine()
    {
        text = textField.text;
        textField.text = text + "9";
    }
    public void Zero()
    {
        text = textField.text;
        textField.text = text + "0";
    }
    public void Point()
    {
        text = textField.text;
        textField.text = text + ".";
    }
    public void Canc()
    {
        text = textField.text;
        string t = null;
        int len = text.Length;
        for (int i = 0; i < len - 1; i++)
        {
            t = t + text[i];
        }
        text = t;
        textField.text = text;
    }
    public void LocalHost()
    {
        textField.text = "localhost";
    }

    // Function to set the IP and close the interface
    public void Enter()
    {
        string text = textField.text;
        baseClient.BCChangeIp(text);
        baseClient.restart = true;
        textInputGo.SetActive(false);
    }
    public void DeactivateInput()
    {
        textInputGo.SetActive(false);
    }
}
