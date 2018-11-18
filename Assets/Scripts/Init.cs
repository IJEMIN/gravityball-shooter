using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
    }
}