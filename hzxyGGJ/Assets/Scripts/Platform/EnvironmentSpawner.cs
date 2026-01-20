using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Tooltip("输入预制体")]
    public List<GameObject> EnvironmentPrefab;
    private bool hasSpawner = false;
    public float xOffest; // 可选：额外的X轴微调偏移

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawner) return;
        if(!collision.CompareTag("Player")) return;

        // 获取当前场景控制器（增加空引用校验）
        var current = GetComponentInParent<EnvironmentController>();
        if (current == null || current.EndPoint == null)
        {
            Debug.LogError("当前场景没有EnvironmentController或EndPoint未赋值！");
            return;
        }

        // 随机选择预制体（增加空列表校验）
        if (EnvironmentPrefab == null || EnvironmentPrefab.Count == 0)
        {
            Debug.LogError("环境预制体列表为空！");
            return;
        }
        GameObject selectGameObject = EnvironmentPrefab[Random.Range(0, EnvironmentPrefab.Count)];
        if (selectGameObject == null)
        {
            Debug.LogError("选中的预制体为空！");
            return;
        }

        // 获取新场景控制器（增加空引用校验）
        EnvironmentController nextScript = selectGameObject.GetComponent<EnvironmentController>();
        if (nextScript == null || nextScript.StartPoint == null)
        {
            Debug.LogError("选中的预制体没有EnvironmentController或StartPoint未赋值！");
            return;
        }

        // ========== 核心修复：计算正确的生成位置 ==========
        // 1. 获取当前场景终点的世界坐标（绝对位置）
        Vector3 currentEndWorldPos = current.EndPoint.position;
        // 2. 获取新预制体起始点 相对于 预制体根对象的本地坐标
        Vector3 nextStartLocalPos = nextScript.StartPoint.localPosition;
        // 3. 新场景的生成位置 = 当前场景终点世界坐标 - 新预制体起始点的本地X坐标（+ 可选偏移）
        Vector3 spawnPos = new Vector3(
            currentEndWorldPos.x - nextStartLocalPos.x + xOffest,
            currentEndWorldPos.y - nextStartLocalPos.y, // Y轴也对齐，防止垂直偏移
            currentEndWorldPos.z - nextStartLocalPos.z
        );

        // 实例化新场景（使用计算好的绝对位置）
        Instantiate(selectGameObject, spawnPos, Quaternion.identity);

        Debug.Log($"生成场景：当前终点坐标 {currentEndWorldPos}，新场景生成坐标 {spawnPos}");
        hasSpawner = true;
    }
}