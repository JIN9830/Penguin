using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    protected GameManager gameManager = GameManager.Instance;
    private GameObject highlight;

    private readonly WaitForSeconds waitForSeconds = new(1.0f);
    private readonly WaitForSeconds waitForHalfSeconds = new(0.5f);

    private void Awake()
    {
        highlight = this.transform.GetChild(0).gameObject;
        highlight.SetActive(false);
    }
    public void ToggleHighLight(bool enable)
    {
        highlight.SetActive(enable);
    }

    public abstract IEnumerator MoveOrder();
}
