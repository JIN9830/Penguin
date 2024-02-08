using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator playerAnimator;
    private int hitCount = 0;
    private int spinCount = 0;

    private void Awake()
    {
        playerAnimator = this.GetComponent<Animator>();
    }
    public void WallHitAnimation()
    {
        playerAnimator.SetTrigger("DeadEyes");
        hitCount++;

        if (hitCount == 2)
        {
            playerAnimator.SetTrigger("CryEyes");
            hitCount = 0;
        }
    }

    public void EdgeHitAniamtion()
    {
        playerAnimator.SetTrigger("SweatRight");
    }

    public void SpinEyesAnimation()
    {
        spinCount++;

        if (spinCount == 8)
        {
            playerAnimator.SetTrigger("SpinEyes");
            spinCount = 0;
        }
    }
}
