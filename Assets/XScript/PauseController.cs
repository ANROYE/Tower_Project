using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

    public string levelName;
    public void pause(int val)
    {
        Time.timeScale = val;
    }
    public void JumpLevel()
    {
        SceneManager.LoadScene("menu");
    }
}
