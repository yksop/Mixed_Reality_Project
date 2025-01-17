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

/// <summary>
/// The BaseClient class is a Unity component that extends the M2MqttUnityClient to provide MQTT communication capabilities.
/// It allows for subscribing to and publishing messages on various MQTT topics, handling received messages, and managing the connection state.
/// The class includes methods for sending and receiving data related to game objects, such as their positions and rotations, as well as handling specific commands and events.
/// It also provides functionality for registering and unregistering topic handlers, and for invoking methods by name.
/// </summary>
namespace M2MqttUnity
{
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

		public RobotController robotController;
		public CapsuleMovement capsuleMovement;
		//public DroppingCandies dCandy;
		public TerrainToggle terrainToggle;
		public DroppingCandies droppingCandies;
		public DataRobotExchange dataRobotExchange;

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

			//Start to collect data from the environment
			dataRobotExchange.StartCoroutine("GetSpatialPoints");

			
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

		// Function to send a call to execute a void function by sending the name of the function to a topic called as the function
		public void SendVoidFunctionCall(string functionName)
		{
			var aa = GetBytesString(functionName.ToCharArray());
			client.Publish("M2MQTT/" + functionName, aa,  MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		// Function that sends the position and the rotation of a game object to a topic with the name of the game object
		public void SendPosRot(GameObject thisObject, Vector3 position, Quaternion rotation)
		{
			if (client == null)
			{
				Debug.LogError("MQTT client is not initialized.");
				return;
			}
            double[] p = new double[] { position[0], position[1], position[2] };
			double[] r = new double[] { rotation[0], rotation[1], rotation[2], rotation[3] };
			var name = thisObject.name;
			byte[] pos = GetBytesBlock(p);
            byte[] rot = GetBytesBlock(r);

			//Debug.Log(thisObject + " Position bytes: " + BitConverter.ToString(pos));
			//Debug.Log(thisObject + " Rotation bytes: " + BitConverter.ToString(rot));

            client.Publish("M2MQTT/" + name + "/position", pos, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
			client.Publish("M2MQTT/" + name + "/rotation", rot, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		// Function that sends the point representing the state of the environment (the room)
		public void SendRoom(string height, byte[] roomPoints)
		{
			//Debug.Log("Room - sending:  M2MQTT/room/" + height + " ---- " + roomPoints.Length/8 + " points: " + BitConverter.ToString(roomPoints));
			client.Publish("M2MQTT/room/" + height, roomPoints, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}
		public void BCChangeIp(string IP)
		{
			brokerAddress = IP;
		}

		// Function that sends the number of the collected items
		public void SendCandyCount(int count)
		{
			byte[] c = BitConverter.GetBytes(count);
			client.Publish("M2MQTT/counter/num", c, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
		}

		// Function that converts an array of doubles to an array of bytes
		static byte[] GetBytesBlock(double[] values)
		{
            return values.SelectMany(value => BitConverter.GetBytes(value)).ToArray();
        }

		// Function that converts an array of chars to an array of bytes
		static byte[] GetBytesString(char[] values)
		{
			var result = new byte[values.Length * sizeof(char)];
			Buffer.BlockCopy(values, 0, result, 0, result.Length);
			return result;
		}

		// Function that converts an array of bytes to an array of doubles
		static double[] GetDoublesBlock(byte[] bytes)
		{
			var result = new double[bytes.Length / sizeof(double)];
			Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
			return result;
		}

		// Function that converts an array of bytes to an array of chars
		static char[] GetStringBlock(byte[] bytes)
		{
			var result = new char[bytes.Length / sizeof(char)];
			Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
			return result;
		}

		// Called when a message is received through MQTT
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

			// Handles the communication on the various topics
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
			
			if (_topic == "M2MQTT/Trajectory")
			{
				// Receives the trajectory that the avatar has to follow
				byte[] beatArray = message;
				
				// Assicurati che la lunghezza dell'array sia un multiplo di 8 (4 byte per float, 2 float per Vector2)
				if (beatArray.Length % 8 == 0)
				{
					int vectorCount = beatArray.Length / 8;
					Vector2[] trajectory = new Vector2[vectorCount];

					for (int i = 0; i < vectorCount; i++)
					{
						float x = BitConverter.ToSingle(beatArray, i * 8);
						float z = BitConverter.ToSingle(beatArray, i * 8 + 4);
						trajectory[i] = new Vector2(x, z);
					}

					// Chiama la funzione per gestire la traiettoria
					//robotController.UpdateTrajectory(trajectory);
					capsuleMovement.SetTrajectory(trajectory);
				}
				else
				{
					Debug.LogError("Il formato dell'array di beat non è corretto.");
				}
			}
			if(_topic == "M2MQTT/counter/reset")
			{
				// Reset the counter
				//dCandy.SetCounter(0);
				droppingCandies.SetCounter(0);
			}
			if (_topic == "M2MQTT/ToggleTerrain")
			{
				// Toggles the lava on the floor
				//Debug.Log("Received toggle lava");
				terrainToggle.ToggleTerrainObject();
			}
			if (_topic == "M2MQTT/ToggleDonuts")
			{
				// Toggles the drop of player rewards by the avatar
				//Debug.Log("Received toggle donuts");
				droppingCandies.ToggleCandyDrop();
			}
			if (_topic == "M2MQTT/ToggleDance")
			{
				// Toggles an animation of the avatar
				//Debug.Log("Received toggle dance");
				robotController.OnHappyButtonPress();
			}
			if (_topic == "M2MQTT/boxcast/low")
			{
				// Changes the height of the low boxcast
				Debug.Log("Received low boxcast change");
				dataRobotExchange.ChangeLowHeight(System.BitConverter.ToSingle(message, 0));
			}
			if (_topic == "M2MQTT/boxcast/high")
			{
				// Changes the height of the high boxcast
				Debug.Log("Received low boxcast change");
				dataRobotExchange.ChangeHighHeight(System.BitConverter.ToSingle(message, 0));
			}
			if (_topic == "M2MQTT/boxcast/bubble")
			{
				// Changes the radius of the bubble to be ignored by boxcast around the player
				Debug.Log("Received low boxcast change");
				dataRobotExchange.ChangeBubble(System.BitConverter.ToSingle(message, 0));
			}
		}

		private void OnDestroy()
		{
			Disconnect();
		}
	}



}


