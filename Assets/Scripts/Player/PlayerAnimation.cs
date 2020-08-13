using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Cached Component
    Animator playerAnimator;
    AnimationClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GameObject.Find("Player Sprite").GetComponent<Animator>();
        clips = playerAnimator.runtimeAnimatorController.animationClips;
    }

    public void PlayAnimation(string name) => playerAnimator.Play(name, -1, 0f);

    public bool IsAnimationPlaying(string name) => playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(name);

    public float GetAnimationLength(string name)
    {
        AnimationClip clip = Array.Find(clips, c => c.name == name);
        if (clip != null)
        {
            return clip.length;
        }
        else
        {
            Debug.LogWarning("Animation" + name + "not Found!");
            return 0f;
        }
    }

    public void SetMove() => playerAnimator.SetTrigger("moveTrigger");

    public void SetDeath() => playerAnimator.SetTrigger("deathTrigger");

}
