using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [개선된 버전] 주기적으로 자동차를 스폰하는 오브젝트 풀링 기반의 스포너입니다.
/// </summary>
public class CarSpawner : MonoBehaviour
{
    [Header("자동차 오브젝트 풀링")]
    [Tooltip("관리할 자동차 오브젝트 목록입니다. 비워두면 실행 시 자식 오브젝트에서 자동으로 찾아 채웁니다.")]
    [SerializeField] private List<CarController> cars = new List<CarController>();
    // [개선] 사용 가능한 자동차를 O(1) 시간 복잡도로 관리하기 위한 큐(Queue)
    private Queue<CarController> availableCars = new Queue<CarController>();

    [Header("스폰 도로 설정")]
    [Tooltip("자동차의 스폰 지점으로 사용될 도로 목록입니다. 각 도로의 첫 번째 자식 오브젝트가 스폰 지점이 됩니다.")]
    [SerializeField] private List<GameObject> roads = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    // [개선] 균일한 스폰을 위해 현재 스폰 지점 인덱스를 추적합니다.
    private int _currentSpawnIndex = 0;

    [Tooltip("자동차 스폰 사이의 대기 시간(초)입니다.")]
    [SerializeField] private float carSpawnDelay = 5;

    [Header("자동차 움직임 설정")]
    [Tooltip("스폰된 자동차가 이동할 거리입니다.")]
    [SerializeField] private float carMovingDistance = 10;

    [Tooltip("스폰된 자동차의 이동 속도입니다.")]
    [SerializeField] private float carMovingSpeed = 8;

    private void Awake()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var car = transform.GetChild(i).GetComponent<CarController>();
            if (car != null)
            {
                cars.Add(car);
                car.gameObject.SetActive(false);
                availableCars.Enqueue(car);
            }
        }


            int roadCount = roads.Count;
            for (int i = 0; i < roadCount; i++)
            {
                GameObject roadStartPoint = roads[i].gameObject.transform.GetChild(0).gameObject;
                spawnPoints.Add(roadStartPoint);
            }
        
    }

    private void Start()
    {
        if (cars.Count > 0 && spawnPoints.Count > 0)
        {
            // [개선] 스폰을 시작하기 전에 스폰 지점 목록을 무작위로 섞습니다.
            ShuffleSpawnPoints();
            StartCoroutine(SpawnCarRoutine());
        }
    }

    /// <summary>
    /// [개선] 큐를 사용하여 효율적으로 자동차를 스폰하는 코루틴입니다.
    /// </summary>
    private IEnumerator SpawnCarRoutine()
    {
        var waitForCarSpawnTime = new WaitForSeconds(carSpawnDelay);

        while (true)
        {
            if (availableCars.Count > 0)
            {
                CarController carToSpawn = availableCars.Dequeue();

                carToSpawn.gameObject.SetActive(true);

                // [개선] 순서대로 스폰 지점을 선택하여 균일한 분포를 보장합니다.
                Transform startPos = spawnPoints[_currentSpawnIndex].transform;
                _currentSpawnIndex++;

                // [개선] 모든 스폰 지점을 한 번씩 다 사용했다면, 인덱스를 초기화하고 목록을 다시 섞습니다.
                if (_currentSpawnIndex >= spawnPoints.Count)
                {
                    _currentSpawnIndex = 0;
                    ShuffleSpawnPoints();
                }

                carToSpawn.StartMove(carMovingDistance, carMovingSpeed, startPos);

                StartCoroutine(ReturnCarWhenDone(carToSpawn));
            }

            yield return waitForCarSpawnTime;
        }
    }

    /// <summary>
    /// [개선] 자동차의 이동이 끝날 때까지 기다린 후, 풀(큐)에 다시 반환하는 코루틴입니다.
    /// </summary>
    private IEnumerator ReturnCarWhenDone(CarController car)
    {
        yield return new WaitUntil(() => !car.isMoving);

        availableCars.Enqueue(car);
    }

    /// <summary>
    /// [개선] 스폰 지점 목록의 순서를 무작위로 섞습니다. (Fisher-Yates shuffle 알고리즘)
    /// </summary>
    private void ShuffleSpawnPoints()
    {
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnPoints.Count);
            GameObject temp = spawnPoints[i];
            spawnPoints[i] = spawnPoints[randomIndex];
            spawnPoints[randomIndex] = temp;
        }
    }
}
