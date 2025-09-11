using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    public bool isMoving = false;
    private Tweener _moveTween;
    private Tweener _timeScaleTween;

    // [추가] 교차로 내부에 있는지 여부를 나타내는 상태 변수
    [SerializeField] private bool _isInIntersection = false;

    // 트윈 중복 생성을 막기 위한 상태 변수
    [SerializeField] private bool _isSlowingDown = false;
    [SerializeField] private bool _isSpeedingUp = false;

    // [추가] 장애물로 인식할 레이어 마스크
    [SerializeField] private LayerMask _obstacleLayer;

    private void Awake()
    {
        // Rigidbody를 Kinematic으로 설정하고 중력을 사용하지 않도록 합니다.
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

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

        // [성능 개선] 물리 쿼리 주기를 조절하기 위한 변수
        var checkInterval = new WaitForSeconds(0.1f); // 0.1초마다 장애물 감지

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
            // [수정] 지정된 _obstacleLayer만 감지하도록 LayerMask 파라미터 추가
            bool isHit = Physics.SphereCast(sphereCastOrigin, carWidth, transform.forward, out hit, rayDistance, _obstacleLayer);

            if (isHit && !_isInIntersection)
            {
                // 플레이어, 다른 차, 또는 신호등이 감지되면 정지합니다.
                // [수정] 교차로 내부에 있을 경우 신호등을 무시합니다.
                bool isTrafficLight = hit.collider.CompareTag("TrafficLight");
                bool shouldStop = hit.collider.CompareTag("Player") || hit.collider.CompareTag("Car") || (isTrafficLight && !_isInIntersection);

                if (shouldStop && !_isSlowingDown)
                {
                    _isSlowingDown = true;
                    _isSpeedingUp = false;

                    // 부드럽게 감속하여 정지
                    _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                    _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 0, 1.0f);
                }
            }
            else if (!_isSpeedingUp) // 장애물이 없으면 다시 부드럽게 가속
            {
                _isSpeedingUp = true;
                _isSlowingDown = false;

                _timeScaleTween?.Kill(); // 이전 트윈이 있다면 종료
                _timeScaleTween = DOTween.To(() => _moveTween.timeScale, x => _moveTween.timeScale = x, 1, 1.0f).SetDelay(0.5f);
            }

            yield return checkInterval; // [성능 개선] 매 프레임 대신 0.1초 대기
        }
    }

    // [추가] 교차로 진입 및 이탈을 감지하는 트리거 이벤트
    private void OnTriggerEnter(Collider other)
    {
        // "Intersection" 태그를 가진 트리거에 진입하면 교차로 내부에 있는 것으로 간주합니다.
        if (other.CompareTag("Intersection"))
        {
            _isInIntersection = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // "Intersection" 태그를 가진 트리거에서 벗어나면 교차로를 벗어난 것으로 간주합니다.
        if (other.CompareTag("Intersection"))
        {
            _isInIntersection = false;
        }
    }
}
