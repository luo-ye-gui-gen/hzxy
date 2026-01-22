using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 排行榜管理器（单例模式），负责管理跑酷游戏前三名分数的存储和获取
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    // 单例实例
    public static LeaderboardManager Instance { get; private set; }

    // 存储前三名分数的PlayerPrefs键名（可自定义）
    private const string HighScore1Key = "Parkour_HighScore_1";
    private const string HighScore2Key = "Parkour_HighScore_2";
    private const string HighScore3Key = "Parkour_HighScore_3";

    // 最多保留前三名
    private const int MaxRanks = 3;

    // 初始化单例
    private void Awake()
    {
        // 确保全局唯一实例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 获取当前排行榜的前三名分数（已按从高到低排序）
    /// </summary>
    /// <returns>前三名分数列表（不足则补0）</returns>
    public List<int> GetTopScores()
    {
        List<int> scores = new List<int>()
        {
            PlayerPrefs.GetInt(HighScore1Key, 0),
            PlayerPrefs.GetInt(HighScore2Key, 0),
            PlayerPrefs.GetInt(HighScore3Key, 0)
        };

        // 排序（降序），确保即使手动修改数据也能正确排序
        scores.Sort((a, b) => b.CompareTo(a));
        return scores;
    }

    /// <summary>
    /// 添加新分数，自动判断是否进入前三名并更新排行榜
    /// </summary>
    /// <param name="newScore">新的游戏分数（需≥0）</param>
    /// <returns>是否成功进入排行榜</returns>
    public bool AddNewScore(int newScore)
    {
        // 校验分数合法性
        if (newScore < 0)
        {
            Debug.LogError("分数不能为负数！");
            return false;
        }

        // 获取当前前三名分数并添加新分数
        List<int> allScores = GetTopScores();
        allScores.Add(newScore);

        // 去重（可选，避免重复分数占用名额）
        allScores = allScores.Distinct().ToList();

        // 降序排序并截取前三名
        allScores.Sort((a, b) => b.CompareTo(a));
        if (allScores.Count > MaxRanks)
        {
            allScores = allScores.Take(MaxRanks).ToList();
        }

        // 更新PlayerPrefs（持久化）
        for (int i = 0; i < MaxRanks; i++)
        {
            int score = i < allScores.Count ? allScores[i] : 0;
            switch (i)
            {
                case 0:
                    PlayerPrefs.SetInt(HighScore1Key, score);
                    break;
                case 1:
                    PlayerPrefs.SetInt(HighScore2Key, score);
                    break;
                case 2:
                    PlayerPrefs.SetInt(HighScore3Key, score);
                    break;
            }
        }

        PlayerPrefs.Save(); // 强制保存

        // 判断新分数是否进入前三名
        return allScores.Contains(newScore);
    }

    /// <summary>
    /// 清空排行榜数据（测试/重置用）
    /// </summary>
    [ContextMenu("清除分数")]
    public void ClearAllScores()
    {
        PlayerPrefs.DeleteKey(HighScore1Key);
        PlayerPrefs.DeleteKey(HighScore2Key);
        PlayerPrefs.DeleteKey(HighScore3Key);
        PlayerPrefs.Save();
        Debug.Log("排行榜数据已清空");
    }
}