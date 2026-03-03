using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    #region Data

    #endregion

    #region Mono

    void Start()
    {
        Time.timeScale=1.0f;    
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    #endregion 

    #region Methods

    #endregion
}
