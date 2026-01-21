using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageManager : MonoBehaviour
{
    public static MageManager instance;
    public Mage mage;

    void Awake()
    {
        if(instance==null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetMageActive()
    {
        mage.gameObject.SetActive(true);
    }
}
