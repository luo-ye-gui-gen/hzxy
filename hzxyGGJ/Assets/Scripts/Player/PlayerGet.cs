using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGet : MonoBehaviour
{
    private Fire fire;
    // ????????
    private bool isInvincible = false; // ????????
    private float invincibilityTime = 1f; // ???????1??
    private SpriteRenderer playerRenderer; // ??????????????

    void Start()
    {
        fire = GetComponent<Fire>();
        // ?????SpriteRenderer???????????????????????
        playerRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // ?????????????SpriteRenderer?????????
        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ??????
        if (collision.CompareTag("Wood"))
        {
            GameManager.instance.playerHealth.IncreaseHealth();
            Destroy(collision.gameObject);
        }

        // ????????
        if (collision.CompareTag("SmallFire"))
        {
            fire.FireIncrease();
            Destroy(collision.gameObject);
        }

        

        // ?????Ice?GroundCI?
        if ((collision.CompareTag("Ice") || collision.CompareTag("GroundCI")) && !isInvincible)
        {
            // ??
            GameManager.instance.playerHealth.DecreaseHealth();
            // ?????
            Destroy(collision.gameObject);
            if(GameManager.instance.playerHealth.health==0)  return;
            // ?????????
            StartCoroutine(InvincibilityCoroutine());
        }

        
    }

    /// <summary>
    /// ???????
    /// </summary>
    /// <returns></returns>
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true; // ????
        float flashInterval = 0.1f; // ?????0.1?????
        float elapsedTime = 0f;

        // ??????????
        while (elapsedTime < invincibilityTime)
        {
            // ??????????????
            playerRenderer.enabled = !playerRenderer.enabled;
            // ??????
            yield return new WaitForSeconds(flashInterval);
            // ????
            elapsedTime += flashInterval;
        }

        // ?????????????????????
        playerRenderer.enabled = true;
        isInvincible = false;
    }

    // ??????????????????
    private void OnDrawGizmos()
    {
        Gizmos.color = isInvincible ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}