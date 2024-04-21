using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public abstract class CodingBlock : MonoBehaviour
{
    private GameObject _highlight;
    public Tweener BlockTweener { get; protected set; }

    public IObjectPool<CodingBlock> Pool { get; set; }

    private void Awake()
    {
        _highlight = this.transform.GetChild(0).gameObject;
        _highlight.SetActive(false);
    }

    public void ToggleHighLight(bool enable)
    {
        _highlight.SetActive(enable);
    }

    public void ReleaseBlock()
    {
        Pool.Release(this);
    }

    public abstract void MoveOrder();
}
