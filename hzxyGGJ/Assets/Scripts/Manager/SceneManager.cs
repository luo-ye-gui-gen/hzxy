using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);
    }
    // 要加载的目标场景名称（可在Inspector面板手动赋值）
    public string targetSceneName;

    // 点击按钮触发的场景切换方法
    public void SwitchScene()
    {
        // 校验场景名称是否为空
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("目标场景名称未设置！");
            return;
        }

        try
        {
            // 获取当前场景名称（用于日志提示）
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // LoadSceneMode.Single：单场景模式（默认），自动卸载当前场景，加载新场景
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);

            Debug.Log($"已关闭场景【{currentSceneName}】，并加载场景【{targetSceneName}】");
        }
        catch (System.Exception e)
        {
            Debug.LogError("场景切换失败：" + e.Message);
        }
    }
}
