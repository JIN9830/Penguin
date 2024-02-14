using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UIManager
{
    public void ResetBlockAnimation(GameObject blockObj)
    {
        blockObj.transform.DOLocalRotate(new Vector3(0, 180, 0), 1f).SetEase(Ease.Linear)
            .OnComplete(() => blockObj.gameObject.transform.localRotation = Quaternion.Euler(0,0,0));
    }

    public void ForwardBlock_PlayAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        blockObj.gameObject.transform.DOScale(1f,0.5f);
    }

    public void Block_PopAnimation(GameObject blockObj)
    {
        blockObj.gameObject.transform.localScale = Vector3.zero;
        blockObj.gameObject.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
    }
}
