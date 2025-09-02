using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField]
    private List<CarController> _cars = new List<CarController>();

    [SerializeField]
    private List<GameObject> _roads = new List<GameObject>();
    private List<GameObject> _raodStartPosition = new List<GameObject>();

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
            var car = transform.GetChild(i).gameObject.GetComponent<CarController>();
            _cars.Add(car);
            car.gameObject.SetActive(false);
        }

        // .. 도로 오브젝트를 순회하며 도로 시작 지점 오브젝트를 리스트에 추가
        int roadCount = _roads.Count;

        for (int i = 0; i < roadCount; i++)
        {
            GameObject roadsStartPoint = _roads[i].gameObject.transform.GetChild(0).gameObject;
            _raodStartPosition.Add(roadsStartPoint);
        }

    }
    private void Start()
    {
        if (_cars != null)  StartCoroutine(SpawnCar());
    }

    private IEnumerator SpawnCar()
    {
        // 5초 대기는 미리 생성하여 불필요한 메모리 할당을 방지합니다.
        var waitForCarSapwnTime = new WaitForSeconds(_carSpawnDelay);

        while (true)
        {
            // 리스트에 차량이 있는지 먼저 확인합니다.
            if (_cars.Count > 0)
            {
                int carIndex = Random.Range(0, _cars.Count);

                CarController carController = _cars[carIndex];

                // 2. 저장된 변수(carController)를 사용하여 isMoving 프로퍼티에 접근하고 MoveCar() 메서드를 호출합니다.
                //    (컴포넌트가 존재하지 않을 경우를 대비해 null 체크를 추가하는 것이 안전합니다.)
                if (carController != null && !carController.isMoving)
                {
                    carController.gameObject.SetActive(true);
                    carController.MoveCar(_carMovingDistance, _carMovingSpeed, _raodStartPosition[Random.Range(0, _raodStartPosition.Count)].transform);
                }
                
                yield return waitForCarSapwnTime;
            }
            else
            {
                // 리스트가 비어있을 경우, 무한 루프가 CPU를 과도하게 사용하는 것을 막기 위해 
                // 한 프레임 대기 후 다시 시도하도록 합니다.
                yield return null;
            }
        }
    }
}
