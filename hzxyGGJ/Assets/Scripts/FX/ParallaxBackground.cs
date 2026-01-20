using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float xPostion;
    private float length;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        xPostion = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPostion + distanceToMove, transform.position.y);
        if (distanceMoved > xPostion + length)
            xPostion += length;
        else if (distanceMoved < xPostion - length)
            xPostion -= length;
    }
}
