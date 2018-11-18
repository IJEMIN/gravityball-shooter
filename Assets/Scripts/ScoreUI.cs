using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private GameManager m_GameManager;

    public Text m_TimeText;

    private void Start()
    {
        m_GameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_GameManager.m_GameOver) m_TimeText.text = "Time : " + Mathf.Floor(Time.timeSinceLevelLoad);
    }
}