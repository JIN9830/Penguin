using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CodingBlock : MonoBehaviour
{
    protected GameManager Instance = GameManager.Instance;
    public abstract void MoveOrder();
}
