using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Transform coinInitPos;
    private void OnEnable()
    {
        // .. 코인 생성 애니메이션
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1.5f).SetEase(Ease.OutBounce);
    }

    private void Start()
    {
        // .. 코인 IDLE 애니메이션
        gameObject.transform.DORotate(new Vector3(0, -360, 0), 1.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    public void OnTriggerEnter(Collider other)
    {
        // .. 플레이어가 코인에 닿았을 때
        if (other.gameObject.CompareTag("Player"))
        {
            // .. 코인 갯수를 업데이트, 코인 습득 사운드 재생
            BlockCodingManager.StageManager_Instance.UpdateCoin();
            AudioManager.Instance.Play_PlayerSFX("EatCoin");

            // .. 코인 습득 애니메이션 재생
            gameObject.transform.DOMoveY(1, 0.5f).SetEase(Ease.OutBack);
            gameObject.transform.DOScale(0, 1);
        }
    }

}