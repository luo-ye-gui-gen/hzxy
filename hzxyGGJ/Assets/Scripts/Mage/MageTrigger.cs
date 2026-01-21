using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTrigger : MonoBehaviour
{
    public IceType iceType;
    public int RandomI=100;
    public bool NoRandomTime;

    public void SetIce()
    {
        MageManager.instance.mage.SetupIceType(iceType);

        MageManager.instance.SetMageActive();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag == "Player")
        {
            int randomIndex = Random.Range(0,100);

            if (RandomI > randomIndex)
            {
                if (NoRandomTime)
                    SetIce();
                else
                    Invoke("SetIce",Random.Range(0,1.5f));
            }
            
        }
            
    }
}
