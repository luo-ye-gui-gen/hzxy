using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTransform : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("SetActiveFalse",3f);
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
