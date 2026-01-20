using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [Tooltip("ÊäÈëÔ¤ÖÆÌå")]
    public List<GameObject> environmentPrefab = new List<GameObject>();
    private bool hasSpawner = false;

    public int i,k = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawner) return;
        if(!collision.CompareTag("Player")) return;

        var segment = GetComponentInParent<EnvironmentController>();
        if (segment == null || segment.spawnPointsList == null)
        {
            return;
        }
        i = Random.Range(0, segment.spawnPointsList.Count);
        k= i;
        Instantiate(environmentPrefab[k], segment.spawnPointsList[i].position, segment.spawnPointsList[i].rotation);

        hasSpawner = true;
    }
}
