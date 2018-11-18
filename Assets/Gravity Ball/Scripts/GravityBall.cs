using UnityEngine;

public class GravityBall : MonoBehaviour
{
    public enum InputMode
    {
        Swipe,
        Push
    }

    private static GravityBall m_Instance;


    private float angleSVelocity;
    public AudioClip clickClip;

    public InputMode inputMode = InputMode.Swipe;


    private float lastClickAudioPlayTime;

    [SerializeField] private AudioSource m_clickAudioPlayer;

    public float m_ControlRange = 0.08f;

    [SerializeField] private Rigidbody m_coreRigidbody;

    private HapticsController m_HapticsController;

    public string m_HorizontalInputName = "Horizontal";


    public Color m_PressedColor = Color.red;

    private Material m_RenderMaterial;

    private readonly float m_RotationDeadzone = 1.25f;

    [Range(0.1f, 5f)] public float m_RotationSenstive = 1f;

    private Vector3 m_StartPos;
    public string m_VerticalInputName = "Vertical";

    [SerializeField] private Rigidbody m_wraperRigidbody;

    public static GravityBall Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<GravityBall>();

                if (!m_Instance) Debug.LogError("You didn't Crate GravityBall!");
            }

            return m_Instance;
        }
    }


    public bool m_IsTouched { get; private set; }


    private void Awake()
    {
        m_HapticsController = FindObjectOfType<HapticsController>();
    }

    private void Start()
    {
        m_StartPos = m_coreRigidbody.position;

        m_RenderMaterial = GetComponentInChildren<Renderer>().material;
        m_RenderMaterial.SetColor("_EmissionColor", Color.black);
    }

    private void FixedUpdate()
    {
        if (GetButton())
        {
            m_HapticsController.intensityMode = HapticsController.IntensityMode.Strong;
            m_wraperRigidbody.angularVelocity = Vector3.zero;
            m_RenderMaterial.SetColor("_EmissionColor", m_PressedColor);
            return;
        }

        m_HapticsController.intensityMode = HapticsController.IntensityMode.Normal;
        m_RenderMaterial.SetColor("_EmissionColor", Color.black);

        if (m_IsTouched)
        {
            var currentEulerX = m_wraperRigidbody.rotation.eulerAngles.x;

            var deltaAngle = currentEulerX % 6f;

            if (Mathf.Abs(deltaAngle) >= 0.1f)
            {
                Vector3 moveRotation;


                if (m_wraperRigidbody.angularVelocity.x < 0f)
                    moveRotation = new Vector3(-deltaAngle * 0.75f, 0f, 0f);
                else
                    moveRotation = new Vector3(deltaAngle * 0.75f, 0f, 0f);

                transform.Rotate(moveRotation, Space.World);
            }


            if (Time.time >= lastClickAudioPlayTime +
                0.420f / Mathf.Abs(m_wraperRigidbody.angularVelocity.x))
            {
                m_clickAudioPlayer.PlayOneShot(clickClip);
                lastClickAudioPlayTime = Time.time;
                m_HapticsController.PlayTick(0.2f);
            }


            if (Vector3.Distance(m_StartPos, m_coreRigidbody.position) > m_ControlRange)
                m_coreRigidbody.MovePosition(m_StartPos);
        }
    }


    public bool GetButton()
    {
        if (!m_IsTouched) return false;

        if (m_StartPos.y - m_coreRigidbody.position.y >= m_ControlRange * 0.5f) return true;

        return false;
    }


    public float GetInputAxis(string inputName)
    {
        if (!m_IsTouched) return 0f;

        var compress = GetButton() ? 0.2f : 1f;


        if (inputMode == InputMode.Push)
            return GetInputByPush(inputName) * compress;
        return GetInputBySwipe(inputName) * compress;
    }

    private float GetInputByPush(string inputName)
    {
        var deltaVec = m_coreRigidbody.position - m_StartPos;

        var relativeVec = deltaVec / m_ControlRange;

        if (inputName == m_HorizontalInputName) return Mathf.Clamp(relativeVec.x, -1.0f, 1.0f);

        if (inputName == m_VerticalInputName) return Mathf.Clamp(relativeVec.z, -1.0f, 1.0f);

        Debug.LogError("Can't Find Input Name: " + inputName);
        return 0;
    }

    private float GetInputBySwipe(string inputName)
    {
        var angularVelocity = m_wraperRigidbody.angularVelocity;
        angularVelocity *= m_RotationSenstive;


        angularVelocity.x = Mathf.Abs(angularVelocity.x) <= m_RotationDeadzone
            ? 0
            : angularVelocity.x;
        angularVelocity.y = Mathf.Abs(angularVelocity.y) <= m_RotationDeadzone
            ? 0
            : angularVelocity.y;
        angularVelocity.z = Mathf.Abs(angularVelocity.z) <= m_RotationDeadzone
            ? 0
            : angularVelocity.z;


        if (inputName == m_HorizontalInputName)
            return Mathf.Clamp(angularVelocity.z, -1f, 1f);
        if (inputName == m_VerticalInputName) return Mathf.Clamp(angularVelocity.x, -1f, 1f);


        Debug.LogError("Can't Find Input Name: " + inputName);

        return 0;
    }


    private void OnCollisionExit(Collision collision)
    {
        m_IsTouched = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        m_IsTouched = true;
    }


    private void OnDrawGizmos()
    {
        // Draw a wire sphere at each of the points
        Gizmos.color = Color.blue;

        if (m_coreRigidbody == null)
            Gizmos.DrawWireSphere(transform.position, m_ControlRange);
        else
            Gizmos.DrawWireSphere(m_StartPos, m_ControlRange);
    }
}