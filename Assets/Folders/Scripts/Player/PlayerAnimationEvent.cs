using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator playerAnimator;
    private float timer = 0;

    private bool isCry = false;
    private bool isEmbarrass = false;


    //private bool isDizzy = false;
    //private int spinCount = 0;

    private void Awake()
    {
        playerAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if(isCry)
        {
            isCry = AnimationTimer(1.0f);
            playerAnimator.SetBool("IsCry", isCry);
        }
        //else if(isDizzy)
        //{
        //    isDizzy = AnimationTimer(2.3f);
        //    PlayerAnimator.SetBool("IsDizzy", isDizzy);
        //}
        else if(isEmbarrass)
        {
            isEmbarrass = AnimationTimer(1.6f);
            playerAnimator.SetBool("IsEmbarrass", isEmbarrass);
        }
    }


    public void WallHitAnimation() // Event calling location (Death 0:04)
    {
        if(!isCry)
        {
            isCry = true;
        }
    }

    public void EdgeHitAniamtion() // Event calling location (Idle B 0:01)
    {
        if(!isEmbarrass)
        {
            isEmbarrass = true;
        }
    }

    //public void SpinEyesAnimation() // Event calling location (Spin 0:04)
    //{
    //    if (!isDizzy)
    //    {
    //        spinCount++;

    //        if (spinCount == 8)
    //        {
    //            isDizzy = true;
    //            spinCount = 0;
    //        }
    //    }
    //}
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
