using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAnimationTrigger : MonoBehaviour
{
    public bool AnimationTrigger;
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        anim.SetBool("AnimationTrigger",AnimationTrigger);
    }
}
