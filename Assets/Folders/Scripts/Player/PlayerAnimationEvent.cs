using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _playerAnimator;
    private float _timer = 0;

    private bool _isCry = false;
    private bool _isSad = false;

    private void Awake()
    {
        _playerAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isCry)
        {
            _isCry = AnimationTimer(1.6f);
            _playerAnimator.SetBool("IsCry", _isCry);
        }
        else if (_isSad)
        {
            _isSad = AnimationTimer(1.0f);
            _playerAnimator.SetBool("IsSad", _isSad);
        }
    }


    public void HitTheWallEvent() // Event called (Death 0:04)
    {
        _isCry = true;
    }

    public void ReachTheEdgeEvent() // Event called (Idle B 0:01)
    {
        _isSad = true;
    }

    private bool AnimationTimer(float limitTime)
    {
        _timer += Time.deltaTime;

        if (_timer > limitTime)
        {
            _timer = 0;
            return false;
        }

        return true;
    }
}
