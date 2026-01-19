using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerlmd : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("移动设置")]
    [Tooltip("玩家向右的持续移动速度")]
    [SerializeField] private float runSpeed = 5f; // 可在Inspector面板调整
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float currentScore = 0;
    [SerializeField] private float[] accelerateScore = {250,600,1000};//加速时的分数
    [SerializeField] private float[] accelerate = {10,15,20};
    private int accelerateCount = 0;
    private int i = 0;

    [Header("跳跃设置")]
    [Tooltip("跳跃力")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float secondJumpForce = 4f;
    [SerializeField] private Vector2 jumpVelocity;

    [Header("二段跳设置")]
    [Tooltip("最大跳跃次数（2=二段跳，1=普通跳）")]
    [SerializeField] private int maxJumpCount = 2; // 二段跳设为2
    [Tooltip("地面检测射线长度")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [Tooltip("地面图层（需在Inspector指定）")]
    [SerializeField] private LayerMask groundLayer;

    private int currentJumpCount; // 当前剩余跳跃次数
    private bool isGrounded; // 是否在地面

    private void Awake()
    {
        // 获取玩家的Rigidbody2D组件
        rb = GetComponent<Rigidbody2D>();
        // 初始化跳跃次数
        currentJumpCount = maxJumpCount;
    }

    private void FixedUpdate()
    {
        // 保持持续向右的速度
        // 只修改x轴速度，保留y轴速度（用于跳跃/下落）
        currentVelocity = rb.velocity;
        currentVelocity.x = runSpeed; // 固定向右的速度
        rb.velocity = currentVelocity;
        // 检测是否在地面
        CheckGrounded();
    }
    private void Update()
    {
        currentScore += Time.deltaTime * 10;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Accelerate();
    }

    #region 跳跃
    public void Jump()
    {
        // 只有剩余跳跃次数>0时才能跳
        if (currentJumpCount > 0)
        {
            // 保留x轴速度，设置y轴跳跃速度
            jumpVelocity = rb.velocity;
            if(currentJumpCount == 2) jumpVelocity.y = jumpForce;
            else if(currentJumpCount == 1) jumpVelocity.y = secondJumpForce;
            rb.velocity = jumpVelocity;

            // 跳跃后减少剩余次数
            currentJumpCount--;

        }
    }
    #endregion

    # region 跳跃射线检测
    /// <summary>
    /// 检测是否在地面，落地后重置跳跃次数
    /// </summary>
    private void CheckGrounded()
    {
        // 从玩家底部发射短射线检测地面
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0, -0.5f);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);

        // 可视化检测射线（Scene视图中可见）
        Debug.DrawRay(checkPos, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red);

        isGrounded = hit;

        // 落地后重置跳跃次数
        if (isGrounded)
        {
            currentJumpCount = maxJumpCount;
        }
    }

    // 选中物体时绘制直线Gizmos（编辑模式可见）
    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0, -0.5f);
        Vector2 checkEndPos = checkPos + Vector2.down * groundCheckDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(checkPos, checkEndPos); // 绘制检测直线
        Gizmos.DrawWireSphere(checkPos, 0.05f); // 起点小原点（可选）
    }
    #endregion

    #region 加速
    public void Accelerate()

    {
        if (i <= 2 && currentScore >= accelerateScore[i] && accelerateCount == i)
        {
            accelerateCount++;
            runSpeed = accelerate[i];
            i++;
        }
    }
    #endregion
}
