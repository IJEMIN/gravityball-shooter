using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public GameObject m_GameOverCam;
	public GameObject m_GameOverUI;
    public bool m_GameOver { get; private set; }
    void Start()
    {
        m_GameOverCam.SetActive(false);
        m_GameOver = false;
		m_GameOverUI.SetActive(false);
        FindObjectOfType<Player>().GetComponent<LivingEntity>().OnDeath += () =>
        {
            m_GameOver = true;
            m_GameOverCam.SetActive(true);
			m_GameOverUI.SetActive(true);
        };
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene(0);

        }

        if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
