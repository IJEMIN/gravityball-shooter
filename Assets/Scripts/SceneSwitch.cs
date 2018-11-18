using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SceneManager.LoadScene(2);
    }
}