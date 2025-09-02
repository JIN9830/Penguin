using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private List<CarController> _cars = new List<CarController>();

    [SerializeField]
    private List<GameObject> _roads = new List<GameObject>();
    private List<GameObject> _roadStartPosition = new List<GameObject>();

    [SerializeField]
    private int _carSpawnDelay = 5;
    [SerializeField]
    private int _carMovingDistance = 10;
    [SerializeField]
    private float _carMovingSpeed = 8;

    private void Awake()
    {
        // .. 본인의 자식 오브젝트를 순회하며 자동차 오브젝트를 리스트에 추가
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            var car = transform.GetChild(i).GetComponent<CarController>();
            if (car != null)
            {
                _cars.Add(car);
                car.gameObject.SetActive(false);
            }
        }

        // .. 도로 오브젝트를 순회하며 도로 시작 지점 오브젝트를 리스트에 추가
        int roadCount = _roads.Count;

        for (int i = 0; i < roadCount; i++)
        {
            GameObject roadStartPoint = _roads[i].gameObject.transform.GetChild(0).gameObject;
            _roadStartPosition.Add(roadStartPoint);
        }
    }

    private void Start()
    {
        if (_cars.Count > 0) StartCoroutine(SpawnCar());
    }

    private IEnumerator SpawnCar()
    {
        var waitForCarSpawnTime = new WaitForSeconds(_carSpawnDelay);

        while (true)
        {
            CarController availableCar = FindAvailableCar();

            if (availableCar != null)
            {
                availableCar.gameObject.SetActive(true);
                Transform startPos = _roadStartPosition[Random.Range(0, _roadStartPosition.Count)].transform;
                availableCar.MoveCar(_carMovingDistance, _carMovingSpeed, startPos);
            }

            yield return waitForCarSpawnTime;
        }
    }

    private CarController FindAvailableCar()
    {
        for (int i = 0; i < _cars.Count; i++)
        {
            if (!_cars[i].isMoving)
            {
                return _cars[i];
            }
        }
        return null;
    }
}
