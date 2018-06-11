using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{

    public float m_ScrollSpeed = 10f;
    public float m_YawSpeed = 20f;

	public float m_MoveSpeed = 10f;
    public Gun m_Gun;
	public float m_HoverRange = 0.5f;
	private Vector3 m_StartPos;

    private Rigidbody m_GunRigidbody;


	void Start()
	{
		m_StartPos = m_Gun.transform.position;
        m_GunRigidbody = m_Gun.GetComponent<Rigidbody>();

    }
    // Update is called once per frame
    void Update()
    {

        if (GravityBall.Instance.GetButton() || Input.GetMouseButton(0))
        {
            if (m_Gun.m_CurrentState == Gun.State.Ready)
            {
                m_Gun.Fire();
            }
            else if (m_Gun.m_CurrentState == Gun.State.Empty)
            {
                m_Gun.Reload();
            }
        }

        float scroll = GravityBall.Instance.GetInputSwipe("Scroll");
        float yaw = -GravityBall.Instance.GetInputSwipe("Stroke");

        m_GunRigidbody.AddTorque(scroll * m_ScrollSpeed * Time.deltaTime, yaw * m_YawSpeed * Time.deltaTime, 0f);


        float xInput = GravityBall.Instance.GetInputAxis("Horizontal");
        float zIput = GravityBall.Instance.GetInputAxis("Vertical");

        m_Gun.transform.Translate(xInput * m_MoveSpeed * Time.deltaTime, 0, zIput * m_MoveSpeed * Time.deltaTime, Space.World);
		
		if(Vector3.Distance(m_Gun.transform.position,m_StartPos) > m_HoverRange)
		{
			m_Gun.transform.position = m_StartPos + (m_Gun.transform.position - m_StartPos) * m_HoverRange;
		}

    }
}
