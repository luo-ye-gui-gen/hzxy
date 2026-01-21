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

    public void SetMagePlaying()
    {
        MageManager.instance.mage.isPlaying = !MageManager.instance.mage.isPlaying;
    }
    public void PlayMagicAudio()
    {
        AudioManager.instance.PlaySFX(4,null);
    }
}
