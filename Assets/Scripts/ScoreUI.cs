using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreUI : MonoBehaviour
{

    public Text m_TimeText;
    private GameManager m_GameManager;
    void Start()
    {
        m_GameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!m_GameManager.m_GameOver)
        {
            m_TimeText.text = "Time : " + Mathf.Floor(Time.time).ToString();
        }
    }
}
