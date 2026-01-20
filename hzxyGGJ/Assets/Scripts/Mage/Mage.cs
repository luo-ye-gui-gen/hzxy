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
    #region  冰刺横扫配置（右→左）
    [Header("===== ★ 冰刺横扫配置【右→左+跟随玩家Y轴】 ★ =====")]
    public GameObject iceSpikePrefab;       // 拖拽【冰刺】预制体
    public float iceSpawnCd = 3f;           // 冰刺生成间隔（几秒出一次）
    public float iceMoveSpeed = 8f;         // 冰刺横扫速度（越大越快）
    public float yOffsetRange = 1.5f;       // Y轴随机偏移范围（上下各偏移多少，推荐1-2）
    public float yPositionOffsetRange = 1.5f;
    public bool isOpenIceSpike = true;      // 是否开启冰刺功能
    #endregion

    /// <summary>
    /// 核心方法：纯垂直射线检测 + 虚空向右微移重试，无任何圆形检测
    /// 【修改后】冰刺会精准贴合地面生成
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
                // 【关键修改】生成位置改为：检测点的X坐标 + 地面的Y坐标
                // hitGround.point 是射线与地面的交点（精准地面位置）
                Vector2 spawnPosition = new Vector2(rayStartPos.x, hitGround.point.y);
                // 生成冰刺，使用地面交点位置
                Instantiate(icePrefab, spawnPosition+new Vector2(0,1.48f+1.720213f), Quaternion.identity);
                return;
            }

            // ? 检测到虚空 → 向右(向前)移动一小步，更新检测点，继续检测
            currentCheckX += rightStep;
            rayStartPos = new Vector2(currentCheckX, player.position.y);
        }
        // 向右检测完所有步数都没地面 → 不生成地刺
    }

    [ContextMenu("右边生成冰刺")]
    public void CreateIceSpikes()
    {
        int randomAmount = Random.Range(3,5);
        StartCoroutine(CreateIceSpikesCor(randomAmount,1f));
    }


    public IEnumerator CreateIceSpikesCor(int amount,float time)
    {
        for(int i = 0; i < amount; i++)
        {
            CreateIceSpike_Rigidbody();
            yield return new WaitForSeconds(time);
        }
        
    }

    #region 冰刺核心方法 - 右→左全屏横扫 + 跟随玩家Y轴+随机偏移 + 自动销毁
    
    void CreateIceSpike_Rigidbody()
    {
        // 1. 基础位置：屏幕最右侧X轴 + 玩家Y轴（核心修改）
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)); // 屏幕最右侧X轴
        // 核心：以玩家Y轴为基准，加上随机上下偏移
        float randomYOffset = Random.Range(-yOffsetRange+yPositionOffsetRange, yOffsetRange+yPositionOffsetRange);
        spawnPos = new Vector3(spawnPos.x, player.position.y + randomYOffset, 0);

        // 2. 生成冰刺预制体（保留你预制体的旋转/缩放）
        GameObject ice = Instantiate(iceSpikePrefab, spawnPos, Quaternion.identity);

        // 3. 给冰刺加刚体2D（没有就自动加，有就直接用）
        Rigidbody2D iceRb = ice.GetComponent<Rigidbody2D>();
        if (iceRb == null) iceRb = ice.AddComponent<Rigidbody2D>();

        // 4. 刚体关键设置：左移 + 无重力
        iceRb.bodyType = RigidbodyType2D.Kinematic;  // 运动学刚体，无重力、不被碰撞推动
        iceRb.gravityScale = 0;                       // 重力缩放0，防止下坠
        iceRb.velocity = new Vector2(-iceMoveSpeed, 0);// X轴负方向（向左）移动

        // 调整冰刺旋转（匹配向左移动的朝向）
        ice.transform.rotation = Quaternion.Euler(0, 0, 90);
        // 保持原有缩放
        ice.transform.localScale = new Vector3(0.22f, 0.34f, 1);

        // 5. 自动销毁：冰刺移出屏幕左侧后销毁，无内存堆积
        Destroy(ice, 8f);
    }
    #endregion

    // Scene窗口可视化辅助线
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float currentCheckX = player.position.x + startSpawnDistance;
        for (int i = 0; i < maxCheckCount; i++)
        {
            Vector2 rayStart = new Vector2(currentCheckX, player.position.y);
            Gizmos.DrawLine(rayStart, rayStart + Vector2.down * rayDownLength);
            Gizmos.DrawSphere(rayStart, 0.1f);
            currentCheckX += rightStep;
        }

        // 可选：可视化冰刺Y轴偏移范围（方便调试）
        if (player != null)
        {
            Gizmos.color = Color.blue;
            // 玩家Y轴上边界
            Vector3 topPos = new Vector3(player.position.x, player.position.y+yPositionOffsetRange + yOffsetRange, 0);
            // 玩家Y轴下边界
            Vector3 bottomPos = new Vector3(player.position.x, player.position.y+yPositionOffsetRange-yOffsetRange, 0);
            Gizmos.DrawSphere(topPos, 0.2f);
            Gizmos.DrawSphere(bottomPos, 0.2f);
            Gizmos.DrawLine(topPos, bottomPos);
        }
    }
}