using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation
{
    public Tweener Animation_BlockShake(GameObject blockObj)
    {
        Vector3 blockInit = blockObj.transform.localPosition;
        return blockObj.transform.DOShakePosition(1, 10, 10, 0)
            .OnComplete(() => blockObj.transform.localPosition = blockInit);
    }

    public Tweener Animation_ForwardBlockPlay(GameObject blockObj)
    {
        blockObj.transform.localScale = Vector3.zero;
        return blockObj.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
    }

    public Tweener Animation_LeftBlockPlay(GameObject blockObj)
    {
        blockObj.transform.localRotation = Quaternion.identity;
        return blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, 20), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutBounce));
    }

    public Tweener Animation_RightBlockPlay(GameObject blockObj)
    {
        blockObj.transform.localRotation = Quaternion.identity;
        return blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutBounce));
    }

    public Tweener Animation_TimeControl(Button blockObj)
    {
        return blockObj.transform.DORotate(new Vector3(0, 0, -20), 1f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutElastic));
    }

    public Tweener Animation_BlockPop(GameObject blockObj)
    {
        blockObj.transform.localScale = Vector3.zero;
        return blockObj.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutExpo);
    }

    public Tweener Animation_CodingBlockPop(GameObject blockObj)
    {
        return blockObj.transform.DOScale(0.9f, 0.1f).SetEase(Ease.OutCirc)
            .OnComplete(()=> { blockObj.transform.DOScale(0.75f, 0.1f).SetEase(Ease.OutCirc); });
    }

    public Tweener Animation_ButtonDelay(Button blockObj, float delayTime)
    {
        blockObj.interactable = false;
        return blockObj.transform.DOScale(1, 0).SetDelay(delayTime).OnComplete(() => blockObj.interactable = true);
    }

    public Tweener Animation_PlayButtonDelay(Button blockObj, float delayTime)
    {
        blockObj.gameObject.SetActive(true);
        blockObj.transform.localScale = Vector3.zero;
        return blockObj.transform.DOScale(Vector3.one, delayTime).SetEase(Ease.OutExpo);
    }

    public Tweener Animation_UIShake(GameObject blockObj)
    {
        Vector3 blockInit = blockObj.transform.localPosition;
        return blockObj.transform.DOShakePosition(1, 5, 10, 10)
            .OnComplete(() => blockObj.transform.localPosition = blockInit);
    }

    public Tweener Animation_DelayPopUpButton(Button blockObj)
    {
        blockObj.transform.localScale = Vector3.zero;
        blockObj.interactable = false;
        blockObj.gameObject.SetActive(true);
        return blockObj.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutExpo).SetDelay(0.5f).OnComplete(() => blockObj.interactable = true);
    }

    public void Animation_LoadingCurtain(GameObject Upper, GameObject Lower, bool enable)
    {
        if(enable)
        {
            Upper.transform.DOScaleY(21, 1.0f).SetEase(Ease.InOutExpo);
            Lower.transform.DOScaleY(21, 1.0f).SetEase(Ease.InOutExpo);
        }
        else
        {
            Upper.transform.DOScaleY(0, 1.0f).SetEase(Ease.InOutExpo).SetDelay(0.4f);
            Lower.transform.DOScaleY(0, 1.0f).SetEase(Ease.InOutExpo).SetDelay(0.4f);
        }
    }
}
