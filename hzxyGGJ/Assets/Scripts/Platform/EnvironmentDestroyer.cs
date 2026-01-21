using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(15f);
        Destroy(transform.root.gameObject);
    }
}
