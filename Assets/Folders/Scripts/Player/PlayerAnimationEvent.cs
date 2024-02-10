using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private enum FaceAnimations
    {
        isCry,

    }

    private bool isCry = false;
    private bool isDizzy = false;
    private bool isEmbarrass = false;

    private Animator playerAnimator;
    private int spinCount = 0;

    private float timer = 0;

    private void Awake()
    {
        playerAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if(isCry)
        {
            isCry = AnimationTimer(2.7f);
            playerAnimator.SetBool("IsCry", isCry);
        }
        else if(isDizzy)
        {
            isDizzy = AnimationTimer(2.7f);
            playerAnimator.SetBool("IsDizzy", isDizzy);
        }
        else if(isEmbarrass)
        {
            isEmbarrass = AnimationTimer(1.8f);
            playerAnimator.SetBool("IsEmbarrass", isEmbarrass);
        }
    }


    public void WallHitAnimation() // Event calling location (Death 0:04)
    {
        if(!isCry)
        {
            isCry = true;
            playerAnimator.SetBool("IsCry", isCry);
        }
    }

    public void EdgeHitAniamtion()
    {
        if(!isEmbarrass)
        {
            isEmbarrass = true;
        }
    }

    public void SpinEyesAnimation()
    {
        if (!isDizzy)
        {
            spinCount++;

            if (spinCount == 6)
            {
                isDizzy = true;
                spinCount = 0;
            }
        }
    }
    private bool AnimationTimer(float limitTime)
    {
        timer += Time.deltaTime;

        if (timer > limitTime)
        {
            timer = 0;
            return false;
        }

        return true;
    }
}
