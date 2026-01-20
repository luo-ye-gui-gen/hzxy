using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public List<GameObject> hearts = new();
    public Transform heartTransform;
    public GameObject heartPrefab;
    public int maxHealth;
    private int health;
    public bool isAlived;
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
        health--;

        Color newColor = hearts[health].GetComponent<Image>().color;

        newColor.a = 0;

        hearts[health].GetComponent<Image>().color = newColor;

        if(health<=0)
            GameManager.instance.GameOver();
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
        health = 0;
    }
}
