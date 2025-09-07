using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CarController : MonoBehaviour
{
    public bool isMoving = false;
    private Tweener _moveTween;
    private Tweener _timeScaleTween;

    // 트윈 중복 생성을 막기 위한 상태 변수
    [SerializeField] private bool _isSlowingDown = false;
    [SerializeField] private bool _isSpeedingUp = false;

    // 교차로 내부에 있는지 확인하는 플래그
    [SerializeField] private bool isInsideIntersection = false;

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
            // [개선] 3개의 Raycast 대신 SphereCast를 사용하여 물리 쿼리 비용을 줄입니다.
            float carWidth = 0.5f;
            Vector3 sphereCastOrigin = transform.position + (transform.up * 0.5f);
            float rayDistance = 2.0f;

            // Scene 뷰에서 SphereCast를 시각적으로 표시합니다.
            Debug.DrawRay(sphereCastOrigin, transform.forward * rayDistance, Color.red);

            RaycastHit hit;
            bool isHit = Physics.SphereCast(sphereCastOrigin, carWidth, transform.forward, out hit, rayDistance);

            if (isHit)
            {
                // 플레이어나 다른 차가 감지되면 일시정지
                // 교차로 내부에 있을 때는 신호등을 무시합니다.
                bool shouldStop = hit.collider.CompareTag("Player") || hit.collider.CompareTag("Car");
                if (!isInsideIntersection)
                {
                    shouldStop = shouldStop || hit.collider.CompareTag("TrafficLight");
                }
                if (shouldStop && !_isSlowingDown)
                {
                    _isSlowingDown = true;
                    _isSpeedingUp = false;

                    // 부드럽게 감속하여 정지
                    _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                    _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 0, 0.25f);
                }
            }
            else if (!_isSpeedingUp) // 장애물이 없으면 다시 부드럽게 가속
            {
                _isSpeedingUp = true;
                _isSlowingDown = false;

                _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 1, 1.0f);
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 교차로에 진입했는지 확인
        if (other.CompareTag("Intersection"))
        {
            isInsideIntersection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 교차로에서 빠져나왔는지 확인
        if (other.CompareTag("Intersection"))
        {
            isInsideIntersection = false;
        }
    }
}
