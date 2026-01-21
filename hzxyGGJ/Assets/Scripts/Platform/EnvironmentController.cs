using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    

    public Transform StartPoint;
    public Transform EndPoint;

    public float RetunLength()
    {
        return EndPoint.position.x - StartPoint.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.instance.playerHealth.Die();
    }
}
