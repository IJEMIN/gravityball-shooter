using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBall : MonoBehaviour
{
    public static GravityBall Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<GravityBall>();

                if (!m_Instance)
                {
                    Debug.LogError("You didn't Crate GravityBall!");
                }
            }
            return m_Instance;
        }
    }

    private static GravityBall m_Instance;

    public LayerMask m_ContactTarget;
    //private SphereCollider m_Collider;
    private const float m_ColliderThickness = 0.001f;

    public bool m_IsTouched { get; private set; }

    public HapticsController m_HapticsController;

    private Vector3 m_StartPos;

    private Material m_RenderMaterial;

    private Rigidbody m_Rigidbody;

    public Color m_IdleColor = Color.black;
    public Color m_PressedColor = Color.red;

    public string m_XDistanceInputName = "Horizontal";
    public string m_ZDistanceINputName = "Vertical";

    public string m_XSwipeInputName = "Scroll";
    public string m_YSwipeInputName = "Spin";
    public string m_ZSwipeInputName = "Stroke";

    public float m_ControlRange = 0.08f;

    [Range(0.01f, 0.2f)]
    public float m_RotationDeadzone = 0.01f;

    [Range(0.1f, 5f)]
    public float m_RotationSenstive = 1f;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_HapticsController = FindObjectOfType<HapticsController>();
    }

    private void Start()
    {
        m_StartPos = m_Rigidbody.position;

        m_RenderMaterial = GetComponentInChildren<Renderer>().material;
        m_RenderMaterial.color = m_IdleColor;
    }

    private void Update()
    {
        if (GetButton())
        {
            m_RenderMaterial.color = m_PressedColor;
            m_HapticsController.currentFrequency = 200f;
            m_HapticsController.hapticStrength = 1f;
        }
        else
        {
            m_RenderMaterial.color = m_IdleColor;

            m_HapticsController.currentFrequency = 164f;
            m_HapticsController.hapticStrength = 0.8f;
        }
    }

    private void FixedUpdate()
    {
        // 범위를 벗어난 경우 범위 내부로 강제로 되돌리기
        if (m_IsTouched)
        {
            if (Vector3.Distance(m_StartPos, m_Rigidbody.position) > m_ControlRange)
            {
                m_Rigidbody.MovePosition(m_StartPos + (m_Rigidbody.position - m_StartPos) * m_ControlRange);
            }
        }
    }

    public bool GetButton()
    {
        if (!m_IsTouched)
        {
            return false;
        }

        if ((m_StartPos.y - m_Rigidbody.position.y) >= m_ControlRange * 0.5f)
        {
            return true;
        }

        return false;
    }


    public float GetInputAxis(string inputName)
    {
        if (!m_IsTouched)
        {
            return 0;
        }

        // Swipe를 하고 있다면, 값을 억누르기
        float inputFactorBySwipe = 1.0f;

        if (m_Rigidbody.angularVelocity.sqrMagnitude >= 1.0f)
        {
            inputFactorBySwipe = 1f / m_Rigidbody.angularVelocity.sqrMagnitude;
        }


        Vector3 deltaVec = m_Rigidbody.position - m_StartPos;

        Vector3 relativeVec = deltaVec / m_ControlRange;

        if (inputName == m_XDistanceInputName)
        {
            return Mathf.Clamp(relativeVec.x, -1.0f, 1.0f) * inputFactorBySwipe;
        }
        else if (inputName == m_ZDistanceINputName)
        {
            return Mathf.Clamp(relativeVec.z, -1.0f, 1.0f) * inputFactorBySwipe;
        }

        Debug.LogError("Can't Find Input Name: " + inputName);

        return 0;
    }

    public float GetInputSwipe(string inputName)
    {
        if (!m_IsTouched)
        {
            return 0;
        }

        Vector3 angularVelocity = m_Rigidbody.angularVelocity;
        angularVelocity *= m_RotationSenstive;


        float absX = Mathf.Abs(angularVelocity.x);
        float absY = Mathf.Abs(angularVelocity.y);
        float absZ = Mathf.Abs(angularVelocity.z);

        angularVelocity.x = absX <= m_RotationDeadzone ? 0 : angularVelocity.x;
        angularVelocity.y = absY <= m_RotationDeadzone ? 0 : angularVelocity.y;
        angularVelocity.z = absZ <= m_RotationDeadzone ? 0 : angularVelocity.z;


        if (absX >= absY && absX >= absZ)
        {
            angularVelocity.y = 0;
            angularVelocity.z = 0;

        }
        else if(absY >= absX && absY >= absZ)
        {
            angularVelocity.x = 0;
            angularVelocity.z = 0;
        }
        else if (absZ >= absX && absZ >= absY)
        {
            angularVelocity.x = 0;
            angularVelocity.y = 0;
        }

        float factorByButton = GetButton() ? 0.01f : 1.0f;

        if (inputName == m_XSwipeInputName)
        {
            return Mathf.Clamp(angularVelocity.x, -1f, 1f) * factorByButton;
        }
        else if (inputName == m_YSwipeInputName)
        {
            return Mathf.Clamp(angularVelocity.y, -1f, 1f) * factorByButton;
        }
        else if (inputName == m_ZSwipeInputName)
        {
            return Mathf.Clamp(angularVelocity.z, -1f, 1f) * factorByButton;
        }


        Debug.LogError("Can't Find Input Name: " + inputName);

        return 0;
    }


    private void OnCollisionExit(Collision collision)
    {
        m_IsTouched = false;
        m_Rigidbody.MoveRotation(Quaternion.identity);

    }

    private void OnCollisionStay(Collision collision)
    {
        m_IsTouched = true;
    }


    void OnDrawGizmos()
    {

        // Draw a wire sphere at each of the points
        Gizmos.color = Color.blue;

        if (m_Rigidbody == null)
        {
            Gizmos.DrawWireSphere(transform.position, m_ControlRange);
        }
        else
        {
            Gizmos.DrawWireSphere(m_StartPos, m_ControlRange);

        }
    }
}
