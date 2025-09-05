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
            // 중앙, 왼쪽, 오른쪽 Raycast 시작 위치 계산 (너비 0.5f)
            Vector3 centerRayOrigin = transform.position + (transform.up * 0.5f);
            Vector3 leftRayOrigin = centerRayOrigin - (transform.right * 0.5f);
            Vector3 rightRayOrigin = centerRayOrigin + (transform.right * 0.5f);

            float raydistance = 2.0f;
            // Scene 뷰에서 Raycast를 시각적으로 표시합니다. (빨간색 선)
            Debug.DrawRay(centerRayOrigin, transform.forward * raydistance, Color.red);
            Debug.DrawRay(leftRayOrigin, transform.forward * raydistance, Color.red);
            Debug.DrawRay(rightRayOrigin, transform.forward * raydistance, Color.red);

            RaycastHit hit;
            // 3개의 Raycast 중 하나라도 감지되면 true
            bool isHit = Physics.Raycast(centerRayOrigin, transform.forward, out hit, raydistance) ||
                         Physics.Raycast(leftRayOrigin, transform.forward, out hit, raydistance) ||
                         Physics.Raycast(rightRayOrigin, transform.forward, out hit, raydistance);

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
