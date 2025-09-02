using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private List<CarController> cars = new List<CarController>();

    [SerializeField]
    private List<GameObject> roads = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    [SerializeField]
    private int carSpawnDelay = 5;
    [SerializeField]
    private int carMovingDistance = 10;
    [SerializeField]
    private float carMovingSpeed = 8;

    private void Awake()
    {
        // .. 본인의 자식 오브젝트를 순회하며 자동차 오브젝트를 리스트에 추가
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var car = transform.GetChild(i).GetComponent<CarController>();
            if (car != null)
            {
                cars.Add(car);
                car.gameObject.SetActive(false);
            }
        }

        // .. 도로 오브젝트를 순회하며 도로 시작 지점 오브젝트를 리스트에 추가
        int roadCount = roads.Count;

        for (int i = 0; i < roadCount; i++)
        {
            GameObject roadStartPoint = roads[i].gameObject.transform.GetChild(0).gameObject;
            spawnPoints.Add(roadStartPoint);
        }
    }

    private void Start()
    {
        if (cars.Count > 0) StartCoroutine(SpawnCarRoutine());
    }

    private IEnumerator SpawnCarRoutine()
    {
        var waitForCarSpawnTime = new WaitForSeconds(carSpawnDelay);

        while (true)
        {
            CarController availableCar = FindAvailableCar();

            if (availableCar != null)
            {
                availableCar.gameObject.SetActive(true);
                Transform startPos = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
                availableCar.MoveCar(carMovingDistance, carMovingSpeed, startPos);
            }

            yield return waitForCarSpawnTime;
        }
    }

    private CarController FindAvailableCar()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            if (!cars[i].isMoving)
            {
                return cars[i];
            }
        }
        return null;
    }
}
