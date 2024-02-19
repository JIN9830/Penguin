using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    private GameObject highlight;
    public Tweener blockTweener;
    public bool IsRotating { get; protected set; } = false;

    private void Awake()
    {
        highlight = this.transform.GetChild(0).gameObject;
        highlight.SetActive(false);
    }
    public void ToggleHighLight(bool enable)
    {
        highlight.SetActive(enable);
    }

    
    public abstract void MoveOrder();

    public virtual IEnumerator Subroutine()
    {
        yield return null;
    }
}
