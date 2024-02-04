using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    protected GameManager gameManager = GameManager.Instance;
    public GameObject highlight;
    public abstract void MoveOrder();

    private void Awake()
    {
        highlight = this.transform.GetChild(0).gameObject;
        highlight.SetActive(false);
    }
    public void ToggleHighLight(bool enable)
    {
        highlight.SetActive(enable);
    }
}
