using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager
{
    public Tweener BlockShakeAnimation(GameObject blockObj)
    {
        Vector3 blockInit = blockObj.transform.localPosition;
        Tweener blockTweener = blockObj.transform.DOShakePosition(1f, 10, 10, 0)
            .OnComplete(()=> blockObj.transform.localPosition = blockInit);
        return blockTweener;
    }

    public Tweener ForwardBlock_PlayAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        Tweener blockTweener = blockObj.gameObject.transform.DOScale(1f,0.5f).SetEase(Ease.OutExpo);
        return blockTweener;
    }

    public Tweener LeftBlock_PlayAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Tweener blockTweener = blockObj.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(()=> blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.4f).SetEase(Ease.OutBounce));
        return blockTweener;
    }

    public Tweener RightBlock_PlayAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        Tweener blockTweener = blockObj.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.6f).SetEase(Ease.OutElastic)
            .OnComplete(() => blockObj.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.4f).SetEase(Ease.OutBounce));
        return blockTweener;
    }

    public void Block_PopAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        blockObj.gameObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutExpo);
    }

    public Tweener SpeedBtn_Animation(GameObject btnObj)
    {
        btnObj.transform.localScale = Vector3.one;
        Tweener btnTweener = btnObj.transform.DOScale(1.1f, 0.5f).SetEase(Ease.OutExpo).SetLoops(int.MaxValue,LoopType.Yoyo);
        return btnTweener;
    }

}
