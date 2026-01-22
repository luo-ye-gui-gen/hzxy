using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Button startButton;
    public static StartButton instance;

    void Awake()
    {
        if (instance == null && instance != this)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void OnEnable()
    {
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        startButton.gameObject.SetActive(true);
    }
    public void OnStartButtonClick()
    {
        SceneManager.instance.SwitchScene(SceneManager.instance.targetSceneName[1]);

    }
}
