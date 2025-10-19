using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 교차로의 신호등 시스템을 제어합니다.
/// </summary>
public class TrafficLightController : MonoBehaviour
{
    [Header("신호등 그룹")]
    [Tooltip("서로 마주보는 신호등 그룹입니다. (예: 1, 3번 도로)")]
    [SerializeField] private List<GameObject> trafficLightGroupA;

    [Tooltip("나머지 마주보는 신호등 그룹입니다. (예: 2, 4번 도로)")]
    [SerializeField] private List<GameObject> trafficLightGroupB;

    [Header("신호 주기 설정")]
    [Tooltip("신호가 바뀌는 주기(초)입니다.")]
    [SerializeField] private float switchInterval = 10f;

    private bool isGroupA_Green = true;

    private void Start()
    {
        // 코루틴을 시작하여 주기적으로 신호를 변경합니다.
        StartCoroutine(TrafficLightRoutine());
    }

    private IEnumerator TrafficLightRoutine()
    {
        while (true)
        {
            // 현재 신호 상태에 따라 신호등 그룹을 제어합니다.
            if (isGroupA_Green)
            {
                // A그룹 초록불, B그룹 빨간불
                SetTrafficLightState(trafficLightGroupA, true);
                SetTrafficLightState(trafficLightGroupB, false);
            }
            else
            {
                // A그룹 빨간불, B그룹 초록불
                SetTrafficLightState(trafficLightGroupA, false);
                SetTrafficLightState(trafficLightGroupB, true);
            }

            // 다음 신호 변경까지 대기합니다.
            yield return new WaitForSeconds(switchInterval);

            // 신호 상태를 전환합니다.
            isGroupA_Green = !isGroupA_Green;
        }
    }

    /// <summary>
    /// 지정된 신호등 그룹의 상태를 설정합니다.
    /// </summary>
    /// <param name="group">상태를 변경할 신호등 그룹</param>
    /// <param name="isGreen">초록불로 설정할지 여부 (true: 초록불, false: 빨간불)</param>
    private void SetTrafficLightState(List<GameObject> group, bool isGreen)
    {
        foreach (var light in group)
        {
            // 초록불일 경우 오브젝트를 비활성화(충돌 없음), 빨간불일 경우 활성화(충돌 발생)
            light.SetActive(!isGreen);
        }
    }
}
