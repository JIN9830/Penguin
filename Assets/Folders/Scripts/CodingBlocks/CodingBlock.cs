using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    protected GameManager gameManager = GameManager.Instance;
    public abstract void MoveOrder();
}
