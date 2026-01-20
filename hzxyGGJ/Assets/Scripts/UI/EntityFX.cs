using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;


public class EntityFX : MonoBehaviour
{
    [Header("µ¯³öÎÄ×Ö")]
    [SerializeField] private GameObject popUpTextPrefab;

    public void CreatePopUpText(string _text)
    {
        float randomx = Random.Range(-1, 1);
        float randomy = Random.Range(1, 3);
        Vector3 positionOffest = new Vector3(randomx, randomy,0);
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffest, Quaternion.identity);

        newText.GetComponent<TextMeshPro>().text = _text;
    }

    
}
