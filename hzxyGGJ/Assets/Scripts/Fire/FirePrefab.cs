using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FirePrefab : MonoBehaviour
{
    private float xYelocity;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(xYelocity,0);
    }

    public void SetupFirePrefab(float _xYelocity)
    {
        xYelocity = _xYelocity;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.tag);
        if (collision.tag == "Ice")
        {
            //Ice iceScript = collision.GetComponent<Ice>();

            Destroy(collision.gameObject);

            Destroy(gameObject);
        }
    }

}
