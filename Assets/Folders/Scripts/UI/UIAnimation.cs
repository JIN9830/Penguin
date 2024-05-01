using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation
{
    public Tweener Animation_BlockShake(GameObject blockObj)
    {
        Vector3 blockInit = blockObj.transform.localPosition;
        Tweener blockTweener = blockObj.transform.DOShakePosition(1, 10, 10, 0)
            .OnComplete(() => blockObj.transform.localPosition = blockInit);
        return blockTweener;
    }

    public Tweener Animation_ForwardBlockPlay(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        Tweener blockTweener = blockObj.gameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
        return blockTweener;
    }

    public Tweener Animation_LeftBlockPlay(GameObject blockObj)
    {
        blockObj.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Tweener blockTweener = blockObj.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, 20), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.4f).SetEase(Ease.OutBounce));
        return blockTweener;
    }

    public Tweener Animation_RightBlockPlay(GameObject blockObj)
    {
        blockObj.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Tweener blockTweener = blockObj.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.4f).SetEase(Ease.OutBounce));
        return blockTweener;
    }

    public void Animation_TimeControl(GameObject blockObj)
    {
        blockObj.gameObject.transform.DORotate(new Vector3(0, 0, -20), 1f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.gameObject.transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutElastic));
    }

    public void Animation_BlockPop(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        blockObj.gameObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
    }

    public void Animation_CodingBlockPop(GameObject blockObj)
    {
        blockObj.gameObject.transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutCirc)
            .OnComplete(()=> { blockObj.gameObject.transform.DOScale(0.75f, 0.1f).SetEase(Ease.OutCirc); });
    }

    public void Animation_ButtonDelay(GameObject blockObj, float delayTime)
    {
        blockObj.gameObject.GetComponent<Button>().interactable = false;
        blockObj.gameObject.transform.DOScale(1, 0).SetDelay(delayTime).OnComplete(() => blockObj.gameObject.GetComponent<Button>().interactable = true);
    }

    public void Animation_PlayButtonDelay(GameObject blockObj, float delayTime)
    {
        blockObj.SetActive(true);
        blockObj.gameObject.GetComponent<Button>().interactable = false;
        blockObj.gameObject.transform.localScale = Vector3.zero;
        blockObj.gameObject.transform.DOScale(1, delayTime).SetEase(Ease.OutExpo).OnComplete(() => blockObj.gameObject.GetComponent<Button>().interactable = true);
    }

    public void Animation_UIShake(GameObject blockObj)
    {
        Vector3 blockInit = blockObj.transform.localPosition;
        blockObj.transform.DOShakePosition(1, 5, 10, 10)
            .OnComplete(() => blockObj.transform.localPosition = blockInit);
    }
}
