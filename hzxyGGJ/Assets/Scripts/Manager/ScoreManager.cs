using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("计分配置")]
    public int scorePerSecond = 10;
    public bool isFormatScore = true;

    [Header("UI绑定")]
    public TextMeshProUGUI scoreText;

    private float currentScore;
    public bool isGameRunning = true;
    public int CurrentScore => Mathf.FloorToInt(currentScore);

    private bool isBGM0FadedTo04 = false;
    private bool isBGM1Played = false;

    [Header("BGM切换配置")]
    public int startFadeScore = 900;
    public int switchBGMScore = 1000;
    public float bgm0FadeTo04Time = 1f;
    public float bgm0FadeOutTime = 1f;
    public float bgm1FadeInTime = 2f;
    public float bgm1TargetVolume = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        currentScore = 0;
        UpdateScoreUI();
        isBGM0FadedTo04 = false;
        isBGM1Played = false;
    }

    private void Update()
    {
        if (isGameRunning)
        {
            currentScore += scorePerSecond * Time.deltaTime;
            UpdateScoreUI();
            CheckBGMTrigger();
        }
    }

    private void CheckBGMTrigger()
    {
        int currentIntScore = Mathf.FloorToInt(currentScore);

        // 900分：BGM0从0.8f渐变到0.4f
        if (currentIntScore >= startFadeScore && !isBGM0FadedTo04)
        {
            isBGM0FadedTo04 = true;
            if (AudioManager.instance != null && AudioManager.instance.bgm.Length > 0)
            {
                AudioManager.instance.StartCoroutine(
                    AudioManager.instance.FadeBGM0To04DuringScoreRange(bgm0FadeTo04Time)
                );
            }
        }

        // 1000分：停止BGM0，播放BGM1（从0.08f到0.2f）
        if (currentIntScore >= switchBGMScore && !isBGM1Played)
        {
            isBGM1Played = true;
            if (AudioManager.instance != null && AudioManager.instance.bgm.Length > 1)
            {
                AudioManager.instance.SwitchFromBGM0ToBGM1(bgm0FadeOutTime, bgm1FadeInTime);
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        int showScore = Mathf.FloorToInt(currentScore);
        scoreText.text = isFormatScore ? "分数:" + $"{showScore:n0}" : "分数:" + showScore.ToString();
    }

    public void PauseScore() => isGameRunning = false;
    public void ResumeScore() => isGameRunning = true;

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        isGameRunning = true;
        isBGM0FadedTo04 = false;
        isBGM1Played = false;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAllBGM();
            // 重置后重新播放BGM0（仍以0.8f直接播放）
            AudioManager.instance.PlayBGM(0, false);
        }
    }
}