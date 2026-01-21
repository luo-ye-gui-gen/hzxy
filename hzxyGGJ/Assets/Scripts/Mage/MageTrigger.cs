using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTrigger : MonoBehaviour
{
    public IceType iceType;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
            MageManager.instance.mage.SetupIceType(iceType);
    }
}
