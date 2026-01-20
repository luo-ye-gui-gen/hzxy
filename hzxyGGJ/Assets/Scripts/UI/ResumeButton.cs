using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResumeButton : MonoBehaviour
{
    public static ResumeButton instance;

    [Header("暂停/继续相关按钮")]
    public Button resumeButton;    // 继续暂停游戏按钮（TMPro Button）
    public TextMeshProUGUI countdownText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        resumeButton.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 继续游戏按钮点击事件
    /// </summary>
    public void OnResumeButtonClick()
    {
        if (GameManager.instance.isGamePaused)
        {
            GameManager.instance.Pause();

        }
    }
    
}
