﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;


namespace M2MqttUnity
{
    /// <summary>
    /// M2MQTT that subscribes on a given topic
    /// </summary>
    public class BaseClient : M2MqttUnityClient
    {
        public GameObject MySceneOriginGO;

        public delegate void MessageReceivedDelegate(string topic, string message);
        private Dictionary<string, MessageReceivedDelegate> m_messageHandlers = new Dictionary<string, MessageReceivedDelegate>();

        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        public bool stopConnection = false;
        public bool restart = false;
        public string topic = "M2MQTT_Unity/test";
        public string lastMsg;

        private List<string> eventMessages = new List<string>();

        public void RegisterTopicHandler(string topic, MessageReceivedDelegate messageReceivedDelegate)
        {
            if (!m_messageHandlers.ContainsKey(topic))
            {
                m_messageHandlers.Add(topic, null);
            }

            m_messageHandlers[topic] += messageReceivedDelegate;
        }

        public void UnregisterTopicHandler(string topic, MessageReceivedDelegate messageReceivedDelegate)
        {
            if (m_messageHandlers.ContainsKey(topic))
            {
                m_messageHandlers[topic] -= messageReceivedDelegate;
            }
        }


        //Update method called every frame
        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (stopConnection)
            {
                stopConnection = false;
                Disconnect();
            }

            if (restart) //needs some time
            {
                Disconnect();
                restart = false;
                Connect();
            }

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {

                }
                eventMessages.Clear();
            }
        }


        protected override void OnConnected()
        {
            base.OnConnected();

            if (autoTest)
            {
                //client.Publish(topic, System.Text.Encoding.UTF8.GetBytes("On_Connect message sent on topic "+topic), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                client.Publish("M2MQTT/SetHeightOnStart", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.y.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                //client.Publish("M2MQTT/SetDepthOnStart", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.x.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                //client.Publish("M2MQTT/SetLateralOnStart", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.z.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                client.Publish("M2MQTT/SecondTopic", System.Text.Encoding.UTF8.GetBytes("3.44"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

            }
        }


        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { topic });
        }

        public void ExampleSending()
        {
            double[] variablestest = new double[2];
            variablestest[0] = 1.3;
            variablestest[1] = 2.1;
            var aa = GetBytesBlock(variablestest);

            client.Publish("M2MQTT/Poses", aa, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

        static byte[] GetBytesBlock(double[] values)
        {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        static double[] GetDoublesBlock(byte[] bytes)
        {
            var result = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
            return result;
        }


        protected override void DecodeMessage(string _topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            Debug.Log(" on topic:" + _topic);

            if (_topic == "M2MQTT/Poses")
            {
                double[] output = GetDoublesBlock(message);
                double test_1 = 0;
                double test_2 = 0;

                for (int j = 0; j < output.Length; j++)
                {
                    if (j == 0) test_1 = output[j];
                    if (j == 1) test_2 = output[j];
                }

                Debug.Log("test_1" + test_1);
                Debug.Log("test_2" + test_2);


            }

            if (_topic == "M2MQTT/SecondTopic")
            {
                Debug.Log("Antonio Cassano il Re della Roma\nmessage ----> " + msg);
            }

            /*           foreach (string topicKey in m_messageHandlers.Keys)
                      {
                          //if (m_messageHandlers.ContainsKey(_topic))
                          if (_topic.Contains(topicKey))
                          {
                              MessageReceivedDelegate messageReceivedDelegate = m_messageHandlers[topicKey];
                              if (messageReceivedDelegate != null)
                              {
                                  messageReceivedDelegate(_topic, msg);
                              }
                          }
                      } */
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }



}


