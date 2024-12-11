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
    public GameObject avatar;
    //public Material lineMaterial;
    private Color lineColor = Color.green;

    public void PlayerUpdatePosition(byte[] posB)
    {
        Debug.Log(posB);
        Vector3 pos = new Vector3(posB[0], posB[1], posB[2]);
        player.transform.position = pos * 20;
    }
    public void PlayerUpdateRotation(byte[] rotB)
    {
        Quaternion rot = new Quaternion(rotB[0], rotB[1], rotB[2], rotB[3]);
        player.transform.rotation = rot;
    }

    public void AvatarUpdatePosition(byte[] posB)
    {
        Vector3 pos = new Vector3(posB[0], posB[1], posB[2]);
        avatar.transform.position = pos * 20;
    }
    public void AvatarUpdateRotation(byte[] rotB)
    {
        Quaternion rot = new Quaternion(rotB[0], rotB[1], rotB[2], rotB[3]);
        avatar.transform.rotation = rot;
    }
    /*
    public void DrawRoom(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = points[i];
            LineRenderer lr = myLine.AddComponent(typeof(LineRenderer)) as LineRenderer;
            lr.material = lineMaterial;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, points[i]);
            lr.SetPosition(1, points[i+1]);
        }
    }
    public void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lr.material = lineMaterial;
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
    */
}