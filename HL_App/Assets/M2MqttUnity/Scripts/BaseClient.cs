using System;
using System.Reflection;
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
		//public PlayerVisualizer playerVisualizer;

		public delegate void MessageReceivedDelegate(string topic, string message);
		private Dictionary<string, MessageReceivedDelegate> m_messageHandlers = new Dictionary<string, MessageReceivedDelegate>();

		[Tooltip("Set this to true to perform a testing cycle automatically on startup")]
		public bool autoTest = false;
		public bool stopConnection = false;
		public bool restart = false;
		public string topic = "M2MQTT_Unity/test";
		public string lastMsg;
		static public Vector2 absPosition;

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
				Debug.Log("Auto test enabled");
				//client.Publish("M2MQTT/TestTopic", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.y.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
				//client.Publish("M2MQTT/TestTopic", System.Text.Encoding.UTF8.GetBytes("3.44"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
				//client.Publish("M2MQTT/SetDepthOnStart", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.x.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
				//client.Publish("M2MQTT/SetLateralOnStart", System.Text.Encoding.UTF8.GetBytes(MySceneOriginGO.transform.localPosition.z.ToString()), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
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

		public void CallMethodByName(string methodName)
		{
			Type type = this.GetType();

			MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				method.Invoke(this, null);
			}
			else
			{
				Debug.Log($"Method {methodName} not found");
			}
		}

		public void SendVoidFunctionCall(string functionName)
		{
			var aa = GetBytesString(functionName.ToCharArray());
			client.Publish("M2MQTT/Avatar", aa, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
			CallMethodByName(functionName);
		}

		public void SendPosRot(GameObject thisObject, Vector3 position, Quaternion rotation)
		{
			double[] p = { position[1], position[2], position[3] };
			double[] r = { rotation[1], rotation[2], rotation[3], rotation[4] };
			var name = GetBytesString(thisObject.name.ToCharArray());
			var pos = GetBytesBlock(p);
			var rot = GetBytesBlock(r);

			client.Publish("M2MQTT/" + name + "/position", pos, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
			client.Publish("M2MQTT/" + name + "/rotation", rot, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		public void SendRoom(byte[] roomPoints)
		{
			client.Publish("M2MQTT/room", roomPoints, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		static byte[] GetBytesBlock(double[] values)
		{
			var result = new byte[values.Length * sizeof(double)];
			Buffer.BlockCopy(values, 0, result, 0, result.Length);
			return result;
		}

		static byte[] GetBytesString(char[] values)
		{
			var result = new byte[values.Length * sizeof(char)];
			Buffer.BlockCopy(values, 0, result, 0, result.Length);
			return result;
		}

		static double[] GetDoublesBlock(byte[] bytes)
		{
			var result = new double[bytes.Length / sizeof(double)];
			Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
			return result;
		}

		static char[] GetStringBlock(byte[] bytes)
		{
			var result = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
			return result;
		}

		protected override void DecodeMessage(string _topic, byte[] message)
		{
			string tmp = "";
			string msg = System.Text.Encoding.UTF8.GetString(message);

			Debug.Log(" on topic:" + _topic);

			char[] output = GetStringBlock(message);
			foreach (char letter in output)
			{
				tmp = tmp + letter;
			}
			Debug.Log(tmp);

			if (_topic == "M2MQTT/player/position")
			{
				//playerVisualizer.PlayerUpdatePosition(message); not needed in hl app
			}
			if (_topic == "M2MQTT/player/rotation")
			{
				//playerVisualizer.PlayerUpdateRotation(message); not needed in hl app
			}
			// if (_topic == "M2MQTT/Avatar")
			// {
			// 	double[] output = GetDoublesBlock(message);

			// 	double test_1 = 2;
			// 	double test_2 = 2;

			// 	for (int j = 0; j < output.Length; j++)
			// 	{
			// 		if (j == 0) test_1 = output[j];
			// 		if (j == 1) test_2 = output[j];
			// 	}

			// 	Debug.Log("test_1 :" + test_1);
			// 	Debug.Log("test_2 :" + test_2);

			// 	absPosition = new Vector2((float)test_1, (float)test_2);
			// }
			if (_topic == "M2MQTT/function")
			{
				foreach (string topicKey in m_messageHandlers.Keys)
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
				}
			}
		}

		private void OnDestroy()
		{
			Disconnect();
		}
	}



}


