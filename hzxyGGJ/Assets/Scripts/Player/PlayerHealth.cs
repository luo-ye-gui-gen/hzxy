using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class PlayerHealth : MonoBehaviour
{
    public List<GameObject> hearts = new();
    public Transform heartTransform;
    public GameObject heartPrefab;
    public int maxHealth;
    public int health;
    public bool isAlived;
    public PlayerAnimator playerAnimator;
    void Awake()
    {
        
        health = maxHealth;
    }

    void OnEnable()
    {
        isAlived = true;
    }

    [ContextMenu("ºı…Ÿ…˙√¸")]
    public void DecreaseHealth()
    {
        if(health==0) return;
        AudioManager.instance.PlaySFX(6,null);
        health--;

        Color newColor = hearts.First().GetComponent<Image>().color;

        newColor.a = 0;

        hearts[health].GetComponent<Image>().color = newColor;

        if(health<=0)
            Die();
    }

    public void IncreaseHealth()
    {
        if(health==maxHealth) return;

        Color newColor = hearts[health].GetComponent<Image>().color;

        newColor.a = 1;

        hearts[health].GetComponent<Image>().color = newColor;

        health++;
    }

    void Start()
    {
        for(int i = 0; i < health; i++)
        {
            hearts.Add(Instantiate(heartPrefab,heartTransform));
        }
    }

    public void ResetHealth()
    {
        health = 2;
    }

    public void Die()
    {
        AudioManager.instance.StopAllBGM();
        isAlived = false;
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.gravityScale=0;
        
        CameraStopOnPlayerDeath.instance.StopCameraMovement();
        ScoreManager.Instance.PauseScore();
        PauseButton.instance.pauseButton.gameObject.SetActive(false);
        AudioManager.instance.PlaySFX(0,null);
    }
}
