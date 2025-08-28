using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class TrafficManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _carObjects = new List<GameObject>();

    [SerializeField]
    private int _carSpawnDelay = 5;

    [SerializeField]
    private int _carMovingDistance = 10;
    [SerializeField]
    private int _carMovingTime = 8;

    private void Awake()
    {
        int childCount = transform.childCount;

        // .. 자식 오브젝트들을 순회하며 자동차 오브젝트를 리스트에 추가
        for (int i = 0; i < childCount; i++)
        {
            var car = transform.GetChild(i).gameObject;
            _carObjects.Add(car);
            car.gameObject.SetActive(false);
        }

    }
    private void Start()
    {
        if (_carObjects != null)  StartCoroutine(CarSpawner());
    }

    private IEnumerator CarSpawner()
    {
        // 5초 대기는 미리 생성하여 불필요한 메모리 할당을 방지합니다.
        var waitForCarSapwnTime = new WaitForSeconds(_carSpawnDelay);

        while (true)
        {
            // 리스트에 차량이 있는지 먼저 확인합니다.
            if (_carObjects.Count > 0)
            {
                int carIndex = Random.Range(0, _carObjects.Count);

                CarController carController = _carObjects[carIndex].transform.GetComponent<CarController>();

                // 2. 저장된 변수(carController)를 사용하여 isMoving 프로퍼티에 접근하고 MoveCar() 메서드를 호출합니다.
                //    (컴포넌트가 존재하지 않을 경우를 대비해 null 체크를 추가하는 것이 안전합니다.)
                if (carController != null && !carController.isMoving)
                {
                    carController.gameObject.SetActive(true);
                    carController.MoveCar(_carMovingDistance, _carMovingTime);
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
