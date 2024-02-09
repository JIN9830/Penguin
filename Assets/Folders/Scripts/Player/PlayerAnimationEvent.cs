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
            isCry = AnimationTimer(3f);
            playerAnimator.SetBool("IsCry", isCry);
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

        }
        //playerAnimator.SetTrigger("IsEmbarrass");
    }

    public void SpinEyesAnimation()
    {
        if (!isDizzy)
        {
            spinCount++;

            if (spinCount == 8)
            {
                playerAnimator.SetTrigger("IsDizzy");
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
