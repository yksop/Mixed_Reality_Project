using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using M2MqttUnity;

public class DonutCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;

    private BaseClient baseClient;
    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (counterText == null)
        {
            Debug.LogError("TextMeshProUGUI non assegnato!");
        }
        counterText.text = "Donuts Eaten: 0" ;
    }

    public void UpdateCounter(int counter)
    {
        if (counterText != null)
        {
            counterText.text = "Donuts Eaten: " + counter.ToString();
        }
        else
        {
            Debug.LogError("TextMeshProUGUI non assegnato!");
        }
    }

    public void ResetCounter()
    {
        counter = 0;
        UpdateCounter(counter);
        //baseClient.SendCounterReset();

    }
}