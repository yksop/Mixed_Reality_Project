using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M2MqttUnity;
using Microsoft.MixedReality.Toolkit.UI;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using Vuforia;
using System.Globalization;

public class MqttMessageHandler : MonoBehaviour
{
    public BaseClient baseClient;
    public GameObject ImageTargetGO;
    public GameObject StonesOriginGO;
    public GameObject MySceneOriginGO;


    private void Start()
    {
        StartVuforiaCamera();
    }

    private void OnEnable()
    {
        /*
        baseClient.RegisterTopicHandler("M2MQTT/CalibrateSharedSpace/Sender/Table", HandleCalibration);
        baseClient.RegisterTopicHandler("M2MQTT/Play", HandlePlay);
        baseClient.RegisterTopicHandler("M2MQTT/Reset", HandleReset);
        baseClient.RegisterTopicHandler("M2MQTT/TOF", HandleTOF);
        baseClient.RegisterTopicHandler("M2MQTT/Height", HandleHeight);
        baseClient.RegisterTopicHandler("M2MQTT/Depth", HandleDepth);
        baseClient.RegisterTopicHandler("M2MQTT/Lateral", HandleLateral);
         */
    }

    private void OnDisable()
    {
        /*
        baseClient.UnregisterTopicHandler("M2MQTT/CalibrateSharedSpace/Sender/Table", HandleCalibration);
        baseClient.UnregisterTopicHandler("M2MQTT/Play", HandlePlay);
        baseClient.UnregisterTopicHandler("M2MQTT/Reset", HandleReset);
        baseClient.UnregisterTopicHandler("M2MQTT/PlayTOF", HandleTOF);
        baseClient.UnregisterTopicHandler("M2MQTT/Height", HandleHeight);
        baseClient.UnregisterTopicHandler("M2MQTT/Depth", HandleDepth);
        baseClient.UnregisterTopicHandler("M2MQTT/Lateral", HandleLateral);
        */
    }

    public void StartAnchorCheck()
    {
        ImageTargetGO.SetActive(true);
        StartVuforiaCamera();
    }

    public void StopAnchorCheck()
    {
        StonesOriginGO.transform.position = ImageTargetGO.transform.position;
        StonesOriginGO.transform.eulerAngles = new Vector3(0, ImageTargetGO.transform.eulerAngles.y - 90.0f, 0);
        ImageTargetGO.SetActive(false);
        StopVuforiaCamera();
    }

    /*
    private void HandleCalibration(string topic, string message)
    {
    
    }

    private void HandleHeight(string topic, string message)
    {
        float messagef = float.Parse(message, NumberStyles.Any, CultureInfo.InvariantCulture);  //convert message to float 
        MySceneOriginGO.transform.localPosition = new Vector3(MySceneOriginGO.transform.localPosition.x, messagef, MySceneOriginGO.transform.localPosition.z);
    }

    private void HandleDepth(string topic, string message)
    {
        float messagef = float.Parse(message, NumberStyles.Any, CultureInfo.InvariantCulture);  //convert message to float 
        MySceneOriginGO.transform.localPosition = new Vector3(messagef, MySceneOriginGO.transform.localPosition.y, MySceneOriginGO.transform.localPosition.z);
    }

    private void HandleLateral(string topic, string message)
    {
        float messagef = float.Parse(message, NumberStyles.Any, CultureInfo.InvariantCulture);  //convert message to float 
        MySceneOriginGO.transform.localPosition = new Vector3(MySceneOriginGO.transform.localPosition.x, MySceneOriginGO.transform.localPosition.y, messagef);
    }

    private void HandlePlay(string topic, string message)
    {

    }

    private void HandleTOF(string topic, string message)
    {

    }

    private void HandleReset(string topic, string message)
    {

    }
    */

        private void StartVuforiaCamera()
    {
        if (!Vuforia.CameraDevice.Instance.IsActive())
        {
            Vuforia.CameraDevice.Instance.Start();
        }
    }

    private void StopVuforiaCamera()
    {
        if (Vuforia.CameraDevice.Instance.IsActive())
        {
            Vuforia.CameraDevice.Instance.Stop();
        }
    }

    //private void StartVuforiaCamera()
    //{
    //    if (!VuforiaBehaviour.Instance.CameraDevice.IsActive)
    //    {
    //        //VuforiaBehaviour.Instance.enabled = true;
    //        Vuforia.CameraDevice.Instance.Start();
    //    }
    //}

    //private void StopVuforiaCamera()
    //{
    //    if (VuforiaBehaviour.Instance.CameraDevice.IsActive)
    //    {
    //        //VuforiaBehaviour.Instance.enabled = false;
    //        Vuforia.CameraDevice.Instance.Stop();
    //    }
    //}

}