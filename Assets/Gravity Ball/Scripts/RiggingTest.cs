using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingTest : MonoBehaviour {

    public Rigidbody rigTarget;
    private Rigidbody myRigid;

    float dampTime = 0.05f;

    float movingSpeed = 1000f;
	// Use this for initialization
	void Start () {
        myRigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //Vector3 direction = rigTarget.position - myRigid.position;
       // transform.position = rigTarget.position;
       // myRigid.AddForce(direction * Time.deltaTime * movingSpeed);

        myRigid.MovePosition(rigTarget.position);
        //myRigid.MoveRotation(rigTarget.rotation);
    }
}
