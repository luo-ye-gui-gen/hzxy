using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGet : MonoBehaviour
{
    private Fire fire;

    void Start()
    {
        fire = GetComponent<Fire>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {

        //¼ñµ½Ä¾Í·¼ÓÑª
        if (collision.tag == "Wood")
        {
            GameManager.instance.playerHealth.IncreaseHealth();

            Destroy(collision.gameObject);
        }

        if(collision.tag == "SmallFire")
        {
            fire.FireIncrease();

            Destroy(collision.gameObject);
        }

        if(collision.tag == "Ice")
        {
            GameManager.instance.playerHealth.DecreaseHealth();

            Destroy(collision.gameObject);
        }

        if(collision.tag == "GroundCI")
        {
            GameManager.instance.playerHealth.DecreaseHealth();

            Destroy(collision.gameObject);
        }

        if(collision.tag == "MageTrigger")
        {
            MageManager.instance.SetMageActive();
        }
    }
}
