using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    public static BackButton instance;
    public Button backButton;
    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        backButton.gameObject.SetActive(false);
    }

    public void OnBackButtonClick()
    {
        if (GameManager.instance.isGamePaused)
            BackStartScene();



    }
    private void BackStartScene()
    {
        SceneManager.instance.SwitchScene(SceneManager.instance.targetSceneName[0]);
    }
}
