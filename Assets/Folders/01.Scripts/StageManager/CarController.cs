using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public bool isMoving = false;
    private Tweener _moveTween;
    private Tweener _timeScaleTween;

    // 트윈 중복 생성을 막기 위한 상태 변수
    private bool _isSlowingDown = false;
    private bool _isSpeedingUp = false;

    public void StartMove(float moveDistance, float moveSpeed, Transform startPos)
    {
        StartCoroutine(MoveCarRoutine(moveDistance, moveSpeed, startPos));
    }

    private IEnumerator MoveCarRoutine(float moveDistance, float moveSpeed, Transform startPos)
    {
        // 초기 위치와 회전 설정
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;

        _isSlowingDown = false;
        _isSpeedingUp = false;

        isMoving = true;

        // 목표 지점 계산 및 이동 시작
        Vector3 targetPosition = transform.position + transform.forward * moveDistance;
        _moveTween = transform.DOMove(targetPosition, moveSpeed).SetSpeedBased().SetEase(Ease.Linear);
        _moveTween.OnComplete(() =>
        {
            isMoving = false;
            // 이동 완료 후 초기화 및 비활성화
            transform.position = startPos.position;
            transform.rotation = startPos.rotation;
            gameObject.SetActive(false);
        });

        // 이동하는 동안 장애물 감지
        while (isMoving)
        {
            // Raycast의 시작 위치를 y축으로 0.5f만큼 올립니다.
            Vector3 rayOrigin = transform.position + (Vector3.up * 0.5f);

            // Scene 뷰에서 Raycast를 시각적으로 표시합니다. (빨간색 선)
            Debug.DrawRay(rayOrigin, transform.forward * 2.0f, Color.red);

            RaycastHit hit;
            // 차 앞 3 유닛에서 Raycast 발사
            if (Physics.Raycast(rayOrigin, transform.forward, out hit, 2.0f))
            {
                // 플레이어나 다른 차가 감지되면 일시정지
                if ((hit.collider.CompareTag("Player") || hit.collider.CompareTag("Car")) && !_isSlowingDown)
                {
                    _isSlowingDown = true;
                    _isSpeedingUp = false;

                    // 부드럽게 감속하여 정지
                    _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                    _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 0, 0.5f);
                }
            }
            else if (!_isSpeedingUp) // 장애물이 없으면 다시 부드럽게 가속
            {
                _isSpeedingUp = true;
                _isSlowingDown = false;

                _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 1, 0.5f);
            }

            yield return null; // 다음 프레임까지 대기
        }
    }
}
