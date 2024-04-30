using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // => 해당 스크립트는 기본 베이스를 프리팹으로 만들고 내부 데이터를 각 스테이지별로 인스펙터에서 관리한다
    [field: SerializeField] public GameObject[] CoinObject { get; private set; }
    [field: SerializeField] public int CoinCount { get; private set; }

    private void Start()
    {
        CoinCount = CoinObject.Length;

        GameManager.GameManager_Instance.Register_StageManager(this.gameObject);
    }

    public void ResetCoin()
    {
        if (CoinCount == CoinObject.Length) 
            return;

        CoinCount = CoinObject.Length;

        foreach(GameObject coin in CoinObject)
        {
            coin.gameObject.SetActive(true);
        }
    }

    public void UpdateCoin() // 코인 스크립트에서 코인이 콜라이더에 닿아 비활성화 될때
    {
        if(CoinCount != 0)
            CoinCount--;

        if (CoinCount == 0)
            StageClear();
    }

    public void StageClear()
    {
        Debug.Log("스테이지 클리어 메서드 작동!");
        Time.timeScale = 1;
        GameManager.CodingUIManager_Instance.ExecutionButton.GetComponent<Button>().interactable= false;
        GameManager.CodingUIManager_Instance.ClearPanel.transform.DOLocalMove(Vector3.zero, 1).SetEase(Ease.OutExpo);
    }

    // 스테이지 상호작용에 필요한 공동 메서드를 작성
    // 해당 공동 메서드는 각 스테이지의 StageManager 인스펙터의 기재된 값을 참조하여 실행된다
}
