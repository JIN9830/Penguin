using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _playerAnimator;
    private float _timer = 0;

    private bool _isHit = false;
    private bool _isSad = false;

    private void Awake()
    {
        _playerAnimator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isHit)
        {
            _isHit = AnimationTimer(1.0f);
            _playerAnimator.SetBool("IsHit", _isHit);
        }
        else if (_isSad)
        {
            _isSad = AnimationTimer(1.0f);
            _playerAnimator.SetBool("IsSad", _isSad);
        }
    }


    public void HitTheWallEvent() // Event called (Death 0:04)
    {
        _isHit = true;
    }

    public void ReachTheEdgeEvent() // Event called (Idle B 0:01)
    {
        _isSad = true;
    }

    public void WalkingSound()
    {
        AudioManager.Instance.Play_PlayerSFX("Walking");
    }

    public void TurningSound()
    {
        AudioManager.Instance.Play_PlayerSFX("Turning");
    }


    public void LandingDustParticle()
    {
        BlockCodingManager.PlayerManager_Instance.LandingDust.Play();
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
