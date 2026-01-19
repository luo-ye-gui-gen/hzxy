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
    public int health;
    void Awake()
    {
        health = maxHealth;
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

    void Start()
    {
        for(int i = 0; i < health; i++)
        {
            hearts.Add(Instantiate(heartPrefab,heartTransform));
        }

        
    }
}
