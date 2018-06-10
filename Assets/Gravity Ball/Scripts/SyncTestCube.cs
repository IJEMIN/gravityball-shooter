using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTestCube : MonoBehaviour {

    public float m_MoveSpeed = 2f;

    public float m_ScrollSpeed = 30f;

    public float m_YawSpeed = 30f;

    private Rigidbody m_Rigidbody;

    private Material m_ColorMaterial;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_ColorMaterial = GetComponent<Renderer>().material;
        m_ColorMaterial.color = Color.white;

    }



    // Update is called once per frame
    void Update () {
       
		if(GravityBall.Instance.GetButton())
        {
            m_ColorMaterial.color = Color.red;
        }
        else
        {
            m_ColorMaterial.color = Color.white;
        }

        float xInput = GravityBall.Instance.GetInputAxis("Horizontal");
        float zIput = GravityBall.Instance.GetInputAxis("Vertical");

        transform.Translate(xInput * m_MoveSpeed * Time.deltaTime, 0, zIput * m_MoveSpeed * Time.deltaTime, Space.World);

        float scroll = GravityBall.Instance.GetInputSwipe("Scroll");
        float yaw = GravityBall.Instance.GetInputSwipe("Stroke");

        transform.Rotate(scroll * m_ScrollSpeed * Time.deltaTime, yaw * m_YawSpeed * Time.deltaTime, 0f,Space.World);
    }
   
}
