using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Tooltip("ÊäÈëÔ¤ÖÆÌå")]
    public GameObject environmentPrefab;
    private bool hasSpawner = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawner) return;
        if(!collision.CompareTag("Player")) return;

        var segment = GetComponentInParent<EnvironmentController>();
        if (segment == null || segment.spawnPoint == null)
        {
            return;
        }

        Instantiate(environmentPrefab,segment.spawnPoint.position,segment.spawnPoint.rotation);

        hasSpawner = true;
    }
}
