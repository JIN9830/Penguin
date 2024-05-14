using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1.5f).SetEase(Ease.OutBounce);
    }

    private void Start()
    {
        gameObject.transform.DORotate(new Vector3(0, -360, 0), 1.5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.StageManager_Instance.UpdateCoin();
            AudioManager.Instance.Play_PlayerSFX("EatCoin");
            this.gameObject.SetActive(false);
        }
    }

}