using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    // 新增：记录上一帧的存活状态，避免重复赋值
    private bool lastIsAlived;

    void Start()
    {
        // 初始化Animator组件
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        // 初始化初始状态
        lastIsAlived = GameManager.instance.playerHealth.isAlived;
        UpdateDeadAnimation();
    }

    void Update()
    {
        // 每帧检测存活状态是否变化
        if (GameManager.instance.playerHealth.isAlived != lastIsAlived)
        {
            lastIsAlived = GameManager.instance.playerHealth.isAlived;
            UpdateDeadAnimation();
        }
    }

    // 封装更新死亡动画的逻辑
    private void UpdateDeadAnimation()
    {
        // 只有Animator组件存在时才赋值，避免空引用报错
        if (animator != null)
        {
            animator.SetBool("Dead", !GameManager.instance.playerHealth.isAlived);
        }
    }

    public void GameOver()
    {
        GameManager.instance.GameOver();
    }
}