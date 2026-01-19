using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : MonoBehaviour
{
    public Transform player;
    #region  主角前方生成冰刺
    [Header("===== 冰刺生成核心配置 =====")]
    public GameObject icePrefab;
    public float startSpawnDistance = 3f;   // 主角正前方【初始检测起点】(往右3米开始检测)
    public float spawnCoolDown = 2f;        // 生成冷却，防重复生成，推荐2秒
    public LayerMask groundLayer;           // 只勾选你创建的Ground地面图层【必赋值】

    [Header("===== 虚空向右检测配置【核心修改?】 =====")]
    public float rightStep = 0.2f;          // 检测到虚空 → 向右(向前)移动的步长，越小越精准
    public int maxCheckCount = 15;          // 最多向右检测多少次，防止无限循环，推荐10-20

    [Header("===== 纯垂直射线配置【无圆形检测?】 =====")]
    public float rayDownLength = 4f;        // 垂直向下射线的长度，推荐3-5米(足够射到地面即可)
    #endregion
    #region  主角左边生成向右的冰刺
    [Header("===== ★ 冰刺横扫配置【新增左→右全屏】 ★ =====")]
    public GameObject iceSpikePrefab;       // 拖拽【冰刺】预制体
    public float iceSpawnCd = 3f;           // 冰刺生成间隔（几秒出一次）
    public float iceMoveSpeed = 8f;         // 冰刺横扫速度（越大越快）
    public float iceYPos = 2f;              // 冰刺生成的Y轴高度（可上下调）
    public bool isOpenIceSpike = true;      // 是否开启冰刺功能
    #endregion

    /// <summary>
    /// 核心方法：纯垂直射线检测 + 虚空向右微移重试，无任何圆形检测
    /// </summary>
    [ContextMenu("生成冰刺")]
    void SpawnSpikeRayCheck()
    {
        // 初始化：检测起点 = 主角X轴 + 初始前方距离，和主角同Y轴高度
        float currentCheckX = player.position.x + startSpawnDistance;
        Vector2 rayStartPos = new Vector2(currentCheckX, player.position.y);

        // 循环检测：虚空→向右挪→再检测
        for (int i = 0; i < maxCheckCount; i++)
        {
            // ? 核心：从当前点 垂直向下 发射2D射线，只检测地面图层
            RaycastHit2D hitGround = Physics2D.Raycast(rayStartPos, Vector2.down, rayDownLength, groundLayer);

            // 检测到地面 → 立刻生成地刺+重置冷却，结束检测
            if (hitGround)
            {
                Instantiate(icePrefab, rayStartPos, Quaternion.identity);
                return;
            }

            // ? 检测到虚空 → 向右(向前)移动一小步，更新检测点，继续检测
            currentCheckX += rightStep;
            rayStartPos = new Vector2(currentCheckX, player.position.y);
        }
        // 向右检测完所有步数都没地面 → 不生成地刺
    }

    #region 冰刺核心方法 - 左→右全屏横扫 + 自动销毁
    [ContextMenu("左边生成冰刺")]
    void CreateIceSpike_Rigidbody()
    {
        // 1. 获取屏幕最左侧的生成位置 + 固定悬浮高度
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        spawnPos = new Vector3(spawnPos.x, iceYPos, 0);

        // 2. 生成冰刺预制体（保留你预制体的旋转/缩放，不用改预制体！）
        GameObject ice = Instantiate(iceSpikePrefab, spawnPos, Quaternion.identity);

        // 3. 给冰刺加刚体2D（没有就自动加，有就直接用）
        Rigidbody2D iceRb = ice.GetComponent<Rigidbody2D>();
        if (iceRb == null) iceRb = ice.AddComponent<Rigidbody2D>();

        // 4. 刚体关键设置【彻底解决往下掉的核心】
        iceRb.bodyType = RigidbodyType2D.Kinematic;  // 运动学刚体，无重力、不被碰撞推动
        iceRb.gravityScale = 0;                       // 重力缩放0，双重保险防止下坠
        iceRb.velocity = new Vector2(iceMoveSpeed, 0);// ?刚体赋值：只向右移动，Y轴速度为0，绝对水平！

        // 生成后再设置旋转
        ice.transform.rotation = Quaternion.Euler(0, 0, -90);
        // 生成后再设置缩放
        ice.transform.localScale = new Vector3(0.5835f, 2.2601f, 1);

        // 5. 自动销毁：冰刺移出屏幕右侧后销毁，无内存堆积
        Destroy(ice, 5f);
    }
    #endregion

    // ? Scene窗口可视化辅助线【超重要】：能看到射线+检测路径，调试超方便
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float currentCheckX = player.position.x + startSpawnDistance;
        for (int i = 0; i < maxCheckCount; i++)
        {
            Vector2 rayStart = new Vector2(currentCheckX, player.position.y);
            // 画垂直向下的射线
            Gizmos.DrawLine(rayStart, rayStart + Vector2.down * rayDownLength);
            // 画检测点的小圆点
            Gizmos.DrawSphere(rayStart, 0.1f);
            // 向右偏移步长
            currentCheckX += rightStep;
        }
    }

    
}

