using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockGravity : MonoBehaviour
{

    public Rigidbody rigidbody;

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }



    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Handletag")
        {

            //If the GameObject has the same tag as specified, output this message in the console
            rigidbody.useGravity = false;
            //Debug.Log("Do something else here");
            Debug.Log("ENTER - COLLISION");
        } 
    }

    void OnCollisionExit(Collision collision)
    {

        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Handletag")
        {

            //If the GameObject has the same tag as specified, output this message in the console
            rigidbody.useGravity = true;
            Debug.Log("EXIT");
        }
    }






}
