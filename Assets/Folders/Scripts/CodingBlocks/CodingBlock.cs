using DG.Tweening;
using System.Collections;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    private GameObject _highlight;
    public Tweener blockTweener;
    public bool IsRotating { get; protected set; } = false;

    private void Awake()
    {
        _highlight = this.transform.GetChild(0).gameObject;
        _highlight.SetActive(false);
    }

    public void ToggleHighLight(bool enable)
    {
        _highlight.SetActive(enable);
    }

    public abstract void MoveOrder();
}
