using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun m_Gun;

    private Rigidbody m_GunRigidbody;
    private Vector3 m_StartPos;
    public float rotationSpeed = 30f;


    private void Start()
    {
        m_StartPos = m_Gun.transform.position;
        m_GunRigidbody = m_Gun.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (GravityBall.Instance.GetButton() || Input.GetMouseButton(0))
        {
            if (m_Gun.m_CurrentState == Gun.State.Ready)
                m_Gun.Fire();
            else if (m_Gun.m_CurrentState == Gun.State.Empty) m_Gun.Reload();
        }

        var horizontalInput = GravityBall.Instance.GetInputAxis("Horizontal");
        var verticalInput = GravityBall.Instance.GetInputAxis("Vertical");

        var xRotation = rotationSpeed * verticalInput * Time.deltaTime;
        var yRotation = -rotationSpeed * horizontalInput * Time.deltaTime;


        m_GunRigidbody.MoveRotation(
            m_GunRigidbody.rotation * Quaternion.Euler(xRotation, yRotation, 0f));
    }
}