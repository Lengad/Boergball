using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public void LoadHostMenu()
    {
        SceneManager.LoadScene("HostMenu");
    }

    public void LoadJoinMenu()
    {
        SceneManager.LoadScene("JoinMenu");
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
}
