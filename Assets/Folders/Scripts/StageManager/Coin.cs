using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{

    private void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("콜라이더 충돌!");
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("콜라이더 조건 달성!");

            GameManager.StageManager_Instance.UpdateCoin();
            this.gameObject.SetActive(false);
        }
    }

}