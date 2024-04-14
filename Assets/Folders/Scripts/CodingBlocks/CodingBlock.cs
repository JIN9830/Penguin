using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public abstract class CodingBlock : MonoBehaviour
{
    private IObjectPool<CodingBlock> _ManagedPool;

    private GameObject highlight;
    public Tweener blockTweener;
    public bool IsRotating { get; protected set; } = false;

    private void Awake()
    {
        highlight = this.transform.GetChild(0).gameObject;
        highlight.SetActive(false);
    }

    public void SetManagedPool(IObjectPool<CodingBlock> pool)
    {
        _ManagedPool = pool;
    }

    public void DestroyBlock()
    {
        _ManagedPool.Release(this);
    }

    public void ToggleHighLight(bool enable)
    {
        highlight.SetActive(enable);
    }

    public virtual void MoveOrder()
    {
        GameManager.GameManager_Instance.codingBlockState = GameManager.ECodingBlockState.Playing;
    }
}
