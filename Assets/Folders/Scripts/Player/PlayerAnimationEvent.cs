using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator playerAnimator;
    private float timer = 0;

    private bool isCry = false;
    private bool isSad = false;

    private void Awake()
    {
        playerAnimator = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(isCry)
        {
            isCry = AnimationTimer(1.6f);
            playerAnimator.SetBool("IsCry", isCry);
        }
        else if(isSad)
        {
            isSad = AnimationTimer(1.2f);
            playerAnimator.SetBool("IsSad", isSad);
        }
    }


    public void HitTheWall() // Event calling location (Death 0:04)
    {
        if(!isCry)
        {
            isCry = true;
        }
    }

    public void EdgeHitAniamtion() // Event calling location (Idle B 0:01)
    {
        if(!isSad)
        {
            isSad = true;
        }
    }

    private bool AnimationTimer(float limitTime)
    {
        timer += Time.fixedDeltaTime;

        if (timer > limitTime)
        {
            timer = 0;
            return false;
        }

        return true;
    }
}
