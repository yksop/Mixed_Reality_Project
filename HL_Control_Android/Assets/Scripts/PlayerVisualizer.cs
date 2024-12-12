using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This class is resposnsible for visualizing the position/rotation of the player updated from the HL
*/
public class PlayerVisualizer : MonoBehaviour
{
	Vector3 position;
	Quaternion rotation;
	public GameObject player;
	// Start is called before the first frame update
	void Start()
	{
		player = this.transform.gameObject;
	}

	public void PlayerUpdatePosition(byte[] posB)
	{
		Vector3 pos = new Vector3(posB[1], posB[2], posB[3]);
		player.transform.position = pos;
	}
	public void PlayerUpdateRotation(byte[] rotB)
	{
		Quaternion rot = new Quaternion(rotB[1], rotB[2], rotB[3], rotB[4]);
		player.transform.rotation = rot;
	}
}