using System.Collections;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Animator _playerAnimator;

    private void Awake()
    {
        // GetComponent는 비용이 많이 들 수 있으므로 Awake에서 한 번만 호출합니다.
        _playerAnimator = GetComponent<Animator>();
    }

    // Animation Event: Death (0:04)
    public void HitTheWallEvent()
    {
        StartCoroutine(SetBoolForSeconds("IsHit", 1.0f));
    }

    // Animation Event: Idle B (0:01)
    public void ReachTheEdgeEvent()
    {
        StartCoroutine(SetBoolForSeconds("IsSad", 1.0f));
    }

    public void WalkingSound()
    {
        // AudioManager의 싱글톤 인스턴스를 통해 플레이어 SFX를 재생합니다.
        AudioManager.Instance.Play_PlayerSFX("Walking");
    }

    public void TurningSound()
    {
        AudioManager.Instance.Play_PlayerSFX("Turning");
    }

    public void LandingDustParticle()
    {
        // BlockCodingManager를 통해 PlayerManager의 LandingDust 파티클을 재생합니다.
        GameManager.Instance.PlayerManager.LandingDust.Play();
    }

    /// <summary>
    /// 지정된 시간(초) 동안 Animator의 bool 파라미터를 true로 설정했다가 false로 되돌립니다.
    /// </summary>
    /// <param name="parameterName">Animator의 bool 파라미터 이름</param>
    /// <param name="duration">true로 유지할 시간(초)</param>
    private IEnumerator SetBoolForSeconds(string parameterName, float duration)
    {
        _playerAnimator.SetBool(parameterName, true);
        yield return new WaitForSeconds(duration);
        _playerAnimator.SetBool(parameterName, false);
    }
}
