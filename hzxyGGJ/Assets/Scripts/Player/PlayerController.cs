using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("移动设置")]
    [Tooltip("玩家向右的持续移动速度")]
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private float[] accelerateScore = { 250, 600, 1000 };//加速时的分数阈值
    [SerializeField] private float[] accelerateSpeed = { 10, 15, 20 };//对应分数的移动速度
    private int currentAccelerateLevel = 0; // 当前加速等级（替代原有i和accelerateCount）

    [Header("跳跃设置")]
    [Tooltip("跳跃力")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Vector2 jumpVelocity;
    private float originJumpForce; // 初始跳跃力备份

    [Header("地面检测")]
    [Tooltip("地面检测的偏移量（从物体中心向下）")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0, -0.5f);
    [Tooltip("地面检测射线长度")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [Tooltip("地面图层（需在Inspector指定）")]
    [SerializeField] private LayerMask groundLayer;

    private bool canJump = false; // 单段跳核心：仅落地后为true
    private bool isGrounded = false;

    private void Awake()
    {
        // 获取刚体组件并锁定旋转（防止角色倾斜）
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("玩家物体缺少Rigidbody2D组件！");
            enabled = false;
            return;
        }
        originJumpForce = jumpForce;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // 持续向右移动：仅修改X轴速度，保留Y轴的重力/下落速度
        currentVelocity = rb.velocity;
        currentVelocity.x = runSpeed;
        rb.velocity = currentVelocity;
        
        // 每帧检测地面（物理帧执行更稳定）
        CheckGrounded();
    }

    private void Update()
    {
        // 单段跳触发：仅当按下空格且允许跳跃时执行
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            Jump();
        }

        // 加速逻辑（每帧检测，但通过等级控制只触发一次）
        CheckAccelerate();
    }

    /// <summary>
    /// 单段跳核心逻辑：仅触发一次，落地前无法再次跳跃
    /// </summary>
    private void Jump()
    {
        // 清空Y轴原有速度，避免叠加重力导致跳跃高度不稳定
        jumpVelocity = rb.velocity;
        jumpVelocity.y = 0;
        // 施加跳跃力
        rb.velocity = jumpVelocity;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        
        // 跳完立刻禁用跳跃，直到落地
        canJump = false;
        Debug.Log("执行单段跳");
    }

    /// <summary>
    /// 地面检测：精准判断是否落地，落地后恢复跳跃能力
    /// </summary>
    private void CheckGrounded()
    {
        // 射线起始点：物体中心 + 偏移量（适配不同角色模型）
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        // 发射射线检测地面
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        
        // 调试射线：绿色=落地，红色=空中（Scene视图可见）
        Debug.DrawRay(checkPos, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red);

        // 更新地面状态
        isGrounded = hit;

        // 落地时恢复跳跃能力，空中则禁用
        if (isGrounded)
        {
            if (!canJump) // 仅在从空中落地时触发一次
            {
                canJump = true;
                jumpForce = originJumpForce;
                Debug.Log("落地，恢复跳跃能力");
            }
        }
        else
        {
            canJump = false;
        }
    }

    /// <summary>
    /// 加速逻辑：分数达标后仅触发一次对应等级的加速
    /// </summary>
    private void CheckAccelerate()
    {
        // 安全校验：避免数组越界、ScoreManager为空
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("ScoreManager实例未找到，加速功能暂时失效");
            return;
        }
        if (currentAccelerateLevel >= accelerateScore.Length || currentAccelerateLevel >= accelerateSpeed.Length)
        {
            return; // 已达最高加速等级，无需检测
        }

        // 分数达标且未触发过该等级加速时，执行加速
        if (ScoreManager.Instance.CurrentScore >= accelerateScore[currentAccelerateLevel])
        {
            runSpeed = accelerateSpeed[currentAccelerateLevel];
            currentAccelerateLevel++; // 升级，避免重复触发
            Debug.Log($"加速触发！当前等级：{currentAccelerateLevel}，移动速度：{runSpeed}");
        }
    }

    // Scene视图绘制地面检测辅助线（方便调试）
    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Vector2 checkEndPos = checkPos + Vector2.down * groundCheckDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(checkPos, checkEndPos);
        Gizmos.DrawWireSphere(checkPos, 0.05f); // 射线起点
        Gizmos.DrawWireSphere(checkEndPos, 0.05f); // 射线终点
    }

    // 场景重启/启用时重置参数（防止加速等级残留）
    private void OnEnable()
    {
        currentAccelerateLevel = 0;
        runSpeed = 5f;
        canJump = false; // 初始状态需先落地才能跳
        jumpForce = originJumpForce;
    }

    // 可选：添加2D碰撞器的地面检测兜底（防止射线漏判）
    private void OnCollisionStay2D(Collision2D other)
    {
        // 仅当碰撞的是地面图层，且角色向下运动时（避免墙面碰撞误判）
        if (((1 << other.gameObject.layer) & groundLayer) != 0 && rb.velocity.y <= 0)
        {
            isGrounded = true;
            canJump = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
            canJump = false;
        }
    }
}