using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FirePrefab : MonoBehaviour
{
    private float xYelocity;
    private Rigidbody2D rb;
    private float liveTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(xYelocity,0);

        Destroy(gameObject,liveTime);
    }

    public void SetupFirePrefab(float _xYelocity,float _liveTime)
    {
        xYelocity = _xYelocity;
        liveTime = _liveTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ice")
        {
            Destroy(collision.gameObject);

            Destroy(gameObject);
        }

        if(collision.tag == "Bomb")
        {
            collision.GetComponent<BombExplosion>().SetBombActive();

            Destroy(gameObject);
        }
    }

}
