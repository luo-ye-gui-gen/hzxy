using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public static RestartButton instance;
    public Button restartButton;
    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        restartButton.gameObject.SetActive(false);
    }

    public void OnRestartButtonClick()
    {
        if(GameManager.instance.isGamePaused)
            RestartCurrentScene();
        
        
    }
    private void RestartCurrentScene()
    {
        SceneManager.instance.SwitchScene();
    }
}
