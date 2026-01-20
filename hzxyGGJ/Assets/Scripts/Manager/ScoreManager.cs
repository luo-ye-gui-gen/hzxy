using UnityEngine;
using TMPro;

/// <summary>
/// 地铁酷跑 随时间自动计分核心脚本
/// 挂载：任意全局空物体（如GameManager）
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // 单例模式：全局可以直接调用 ScoreManager.Instance 获取分数管理器
    public static ScoreManager Instance;

    [Header("计分配置")]
    [Tooltip("每秒增加的分数，地铁酷跑默认建议8~15，可自由调整")]
    public int scorePerSecond = 10;  // 核心参数：每秒加分值
    
    [Tooltip("是否开启分数千分位格式化 如：10000 → 10,000")]
    public bool isFormatScore = true;

    [Header("UI绑定")]
    public TextMeshProUGUI scoreText; // 拖拽绑定步骤1创建的ScoreText

    // 当前总分数（私有，外部只读）
    private float currentScore;
    // 游戏是否运行中（控制暂停时停止加分）
    private bool isGameRunning = true;

    // 对外提供只读的分数属性，方便其他脚本调用（比如结算面板）
    public int CurrentScore => Mathf.FloorToInt(currentScore);

    private void Awake()
    {
        // 单例：确保场景中只有一个计分管理器
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
        // 初始化分数和UI显示
        currentScore = 0;
        UpdateScoreUI();
    }

    private void Update()
    {
        // 游戏运行中才加分
        if (isGameRunning)
        {
            // 核心加分逻辑：按时间增量加分，不受帧率影响！！
            // Time.deltaTime 是上一帧到当前帧的时间，乘以每秒加分值 = 每一帧应该加的分数
            currentScore += scorePerSecond * Time.deltaTime;
            
            // 实时更新UI显示
            UpdateScoreUI();
        }
    }

    /// <summary>
    /// 更新分数到UI文本
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText == null) return;
        
        int showScore = Mathf.FloorToInt(currentScore);
        // 格式化分数：带千分位，无小数
        scoreText.text = isFormatScore ? "分数:"+$"{showScore:n0}" : "分数:"+showScore.ToString();
    }

    // 外部调用：暂停游戏/停止加分
    public void PauseScore()
    {
        isGameRunning = false;
    }

    // 外部调用：继续游戏/继续加分
    public void ResumeScore()
    {
        isGameRunning = true;
    }

    // 外部调用：重置分数（重新开始游戏时用）
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
        isGameRunning = true;
    }
}