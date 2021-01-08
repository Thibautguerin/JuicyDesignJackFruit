using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Restart();
        if (Input.GetKeyDown(KeyCode.Escape))
            Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene("SceneTest");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
