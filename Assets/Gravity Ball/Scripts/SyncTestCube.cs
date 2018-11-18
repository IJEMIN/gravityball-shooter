using UnityEngine;

public class SyncTestCube : MonoBehaviour
{
    private Material m_ColorMaterial;
    public float m_MoveSpeed = 2f;

    public float m_ScrollSpeed = 30f;

    public float m_YawSpeed = 30f;

    private void Awake()
    {
        m_ColorMaterial = GetComponent<Renderer>().material;
        m_ColorMaterial.color = Color.white;
    }


    // Update is called once per frame
    private void Update()
    {
        if (GravityBall.Instance.GetButton())
            m_ColorMaterial.color = Color.red;
        else
            m_ColorMaterial.color = Color.white;


        var verticalInput = GravityBall.Instance.GetInputAxis("Vertical");
        var horizontalInput = GravityBall.Instance.GetInputAxis("Horizontal");

        if (GravityBall.Instance.inputMode == GravityBall.InputMode.Push)
            transform.Translate(horizontalInput * m_MoveSpeed * Time.deltaTime, 0,
                verticalInput * m_MoveSpeed * Time.deltaTime,
                Space.World);
        else
            transform.Rotate(verticalInput * m_ScrollSpeed * Time.deltaTime, 0,
                horizontalInput * m_YawSpeed * Time.deltaTime,
                Space.World);
    }
}