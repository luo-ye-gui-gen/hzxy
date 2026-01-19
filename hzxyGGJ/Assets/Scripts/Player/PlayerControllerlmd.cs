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
    [SerializeField] private float[] accelerateScore = {250,600,1000};//加速时的分数
    [SerializeField] private float[] accelerate = {10,15,20};
    private int accelerateCount = 0;
    private int i = 0;

    [Header("跳跃设置")]
    [Tooltip("跳跃力【初始值】")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private Vector2 jumpVelocity;
    private float originJumpForce;

    [Header("二段跳设置")]
    [Tooltip("? 勾选=开启二段跳 | ? 取消=关闭二段跳（变为单段跳）")]
    [SerializeField] private bool isDoubleJumpOpen = true;  // 二段跳总开关【核心新增】
    [Tooltip("最大跳跃次数（2=二段跳，1=普通跳）")]
    [SerializeField] private int maxJumpCount = 2;
    [Tooltip("地面检测射线长度")]
    [SerializeField] private float groundCheckDistance = 0.15f;
    [Tooltip("地面图层（需在Inspector指定）")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("二段跳力度倍率(0~1)")]
    [SerializeField] private float secondJumpRate = 0.8f;

    private int currentJumpCount;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumpCount = maxJumpCount;
        originJumpForce = jumpForce;
        // 获取自身碰撞体，用于射线检测忽略自身
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 持续向右移动 - 只修改X轴，保留Y轴重力/下落速度
        currentVelocity = rb.velocity;
        currentVelocity.x = runSpeed;
        rb.velocity = currentVelocity;
        
        CheckGrounded();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        Accelerate();
    }

    public void Jump()
    {
        // 核心逻辑：判断二段跳开关 + 剩余跳跃次数
        if (currentJumpCount > 0)
        {
            jumpVelocity = rb.velocity;
            // 开启二段跳时：第一段满力跳，第二段按倍率跳
            // 关闭二段跳时：永远都是满力单段跳
            float currentJumpPower = (isDoubleJumpOpen && currentJumpCount == 1) ? jumpForce * secondJumpRate : jumpForce;
            jumpVelocity.y = currentJumpPower;
            rb.velocity = jumpVelocity;

            currentJumpCount--;
        }
    }

    private void CheckGrounded()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0, -0.5f);
        // 修复：射线检测忽略玩家自身的碰撞体，防止误判地面
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(checkPos, Vector2.down * groundCheckDistance, hit ? Color.green : Color.red);

        isGrounded = hit;

        if (isGrounded)
        {
            // 二段跳开关联动：关闭时，重置为1次跳跃机会(单段跳)；开启时，重置为设定的次数(二段跳)
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
        }
    }

    #region 加速
    public void Accelerate()

    {
        if (i <= 2 && ScoreManager.Instance.CurrentScore >= accelerateScore[i] && accelerateCount == i)
        {
            accelerateCount++;
            runSpeed = accelerate[i];
            i++;
        }
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Vector2 checkPos = (Vector2)transform.position + new Vector2(0, -0.5f);
        Vector2 checkEndPos = checkPos + Vector2.down * groundCheckDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(checkPos, checkEndPos);
        Gizmos.DrawWireSphere(checkPos, 0.05f);
        Gizmos.DrawWireSphere(checkEndPos, 0.05f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (groundLayer == (groundLayer | (1 << other.gameObject.layer)))
        {
            // 碰撞落地也联动二段跳开关
            currentJumpCount = isDoubleJumpOpen ? maxJumpCount : 1;
            jumpForce = originJumpForce;
        }
    }
}
