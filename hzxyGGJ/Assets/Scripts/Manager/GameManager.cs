using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerHealth playerHealth;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
        
    }

    public void GameOver()
    {
        Time.timeScale = 0;
    }

}
