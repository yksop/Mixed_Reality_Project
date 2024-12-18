using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Linq;


namespace M2MqttUnity
{
	/// <summary>
	/// M2MQTT that subscribes on a given topic
	/// </summary>
	public class BaseClient : M2MqttUnityClient
	{
		public GameObject MySceneOriginGO;
		public PlayerVisualizer playerVisualizer;
		public PointSpawner pointSpawner;
		public DonutCounter donutCounter;

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
			Debug.Log("Sending on: M2MQTT/" + functionName);
			client.Publish("M2MQTT/" + functionName, aa, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		public void SendPosRot(GameObject thisObject, Vector3 position, Quaternion rotation)
		{
			double[] p = new double[] { position[0], position[1], position[2] };
			double[] r = new double[] { rotation[0], rotation[1], rotation[2], rotation[3] };
			var name = thisObject.name;
			byte[] pos = GetBytesBlock(p);
			byte[] rot = GetBytesBlock(r);
			//Debug.Log(thisObject + "Sending position: " + p[0] + ", " + p[1] + ", " + p[2] + " --- " + pos[0] + ", " + pos[1] + ", " + pos[2]);

			// Debug logs to verify byte array contents
			//Debug.Log(thisObject + " Position bytes: " + BitConverter.ToString(pos));
			//Debug.Log(thisObject + " Rotation bytes: " + BitConverter.ToString(rot));

			client.Publish("M2MQTT/" + name + "/position", pos, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
			client.Publish("M2MQTT/" + name + "/rotation", rot, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		// Function called to reset the donut counter in the HL app
		public void SendCounterReset()
		{
			byte[] res = {1};
			client.Publish("M2MQTT/counter/reset", res, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        }

		public void SendLavaOn()
        {
			byte[] res = { 1 };
			client.Publish("M2MQTT/lava/on", res, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}
		public void SendLavaOff()
		{
			byte[] res = { 1 };
			client.Publish("M2MQTT/lava/off", res, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		public void BCChangeIp(string IP)
		{
			brokerAddress = IP;
		}
		static byte[] GetBytesBlock(double[] values)
		{
			return values.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
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

			//Debug.Log(" on topic:" + _topic);

			char[] output = GetStringBlock(message);
			foreach (char letter in output)
			{
				tmp = tmp + letter;
			}
			//Debug.Log(tmp);

			if (_topic == "M2MQTT/Main Camera/position")
			{
				playerVisualizer.PlayerUpdatePosition(message);
			}
			if (_topic == "M2MQTT/Main Camera/rotation")
			{
				playerVisualizer.PlayerUpdateRotation(message);
			}
			if (_topic == "M2MQTT/Jammo_Player/position")
			{
				playerVisualizer.AvatarUpdatePosition(message);
			}
			if (_topic == "M2MQTT/Jammo_Player/rotation")
			{
				playerVisualizer.AvatarUpdateRotation(message);
			}
			if (_topic == "M2MQTT/room")
			{
				pointSpawner.UpdateMarkers(message);
			}
			if (_topic == "M2MQTT/counter/num")
			{
				donutCounter.UpdateCounter(BitConverter.ToInt32(message, 0));
			}
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


		public void SendTrajectory(Vector2[] trajectoryPoints)
		{
			if (client == null)
			{
				Debug.Log("Client MQTT non è inizializzato!");
				return;
			}

			List<byte> byteList = new List<byte>();

			foreach (Vector2 point in trajectoryPoints)
			{
				byte[] xBytes = BitConverter.GetBytes(point.x);
				byte[] yBytes = BitConverter.GetBytes(point.y);

				byteList.AddRange(xBytes);
				byteList.AddRange(yBytes);
			}

			byte[] byteArray = byteList.ToArray();

			client.Publish("M2MQTT/Trajectory", byteArray, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}



		private void OnDestroy()
		{
			Disconnect();
		}
	}
}
