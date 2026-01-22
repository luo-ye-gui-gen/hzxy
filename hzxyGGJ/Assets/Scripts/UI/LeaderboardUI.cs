using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 排行榜UI展示脚本，挂载到Canvas上
/// </summary>
public class LeaderboardUI : MonoBehaviour
{
    // 拖拽赋值的UI组件
    [Header("UI组件")]
    public Button showLeaderboardBtn; // 查看排行榜按钮
    public GameObject leaderboardPanel; // 排行榜面板
    public TextMeshProUGUI rank1Text; // 第一名文本
    public TextMeshProUGUI rank2Text; // 第二名文本
    public TextMeshProUGUI rank3Text; // 第三名文本

    private void Start()
    {
        // 初始化面板状态（隐藏）
        leaderboardPanel.SetActive(false);

        // 绑定按钮点击事件
        showLeaderboardBtn.onClick.AddListener(ShowLeaderboard);

        // 可选：绑定关闭按钮事件（如果有）
        // closeBtn.onClick.AddListener(HideLeaderboard);
    }

    /// <summary>
    /// 显示排行榜（点击按钮触发）
    /// </summary>
    public void ShowLeaderboard()
    {
        // 从管理器获取前三名分数
        var topScores = LeaderboardManager.Instance.GetTopScores();

        // 更新UI文本（添加名次和样式）
        rank1Text.text = $"历史第一：{topScores[0]} 分";
        rank2Text.text = $"历史第二：{topScores[1]} 分";
        rank3Text.text = $"历史第三：{topScores[2]} 分";

        // 显示面板
        leaderboardPanel.SetActive(true);
    }

    /// <summary>
    /// 隐藏排行榜（可选）
    /// </summary>
    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    // 测试用：添加随机分数（可在Inspector绑定按钮测试）
    public void TestAddRandomScore()
    {
        int randomScore = Random.Range(100, 1000);
        bool isTop3 = LeaderboardManager.Instance.AddNewScore(randomScore);
        Debug.Log($"添加分数：{randomScore}，是否进入前三名：{isTop3}");
    }
}