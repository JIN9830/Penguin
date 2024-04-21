using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using static GameManager;

public class ObjectPoolManager : MonoBehaviour
{
    #region 오브젝트 풀 프리팹

    [System.Serializable]
    public class ObjectInfo
    {
        public string objectName;
        public GameObject prefab;
        public int count;
    }

    [Header("오브젝트 풀 프리팹")]
    public ObjectInfo[] objectInfo;

    #endregion

    [field: Header("코딩블럭 프리팹")]
    [field: SerializeField] public GameObject ForwardPrefab { get; private set; }
    [field: SerializeField] public GameObject TurnLeftPrefab { get; private set; }
    [field: SerializeField] public GameObject TurnRightPrefab { get; private set; }
    [field: SerializeField] public GameObject FunctionPrefab { get; private set; }
    [field: SerializeField] public GameObject LoopPrefab { get; private set; }

    public IObjectPool<CodingBlock> CodingBlockPool { get; private set; }

    [Header("Pool")]
    [SerializeField] private int _initialPoolCapacity = 15;
    [SerializeField] private int _poolMaxSize = 25;

    private void Awake()
    {
        CodingBlockPool = new ObjectPool<CodingBlock>(
            createFunc: CreateBlockObject,
            actionOnGet: OnBlockTakeFromPool,
            actionOnRelease: OnBlockRelease,
            actionOnDestroy: OnBlockDestroy,
            collectionCheck: false,
            defaultCapacity: _initialPoolCapacity,
            maxSize: _poolMaxSize
            );
    }

    private void Start()
    {
        GameManager_Instance.Get_ObjectPoolManager(this.gameObject);
    }

    public CodingBlock CreateBlockObject()
    {
        CodingBlock newBlock = Instantiate(ForwardPrefab).GetComponent<CodingBlock>();
        newBlock.GetComponent<CodingBlock>().Pool = this.CodingBlockPool;
        return newBlock;
    }

    public void OnBlockTakeFromPool(CodingBlock block)
    {
        block.gameObject.SetActive(true);
    }
    public void OnBlockRelease(CodingBlock block)
    {
        block.gameObject.SetActive(false);
    }
    public void OnBlockDestroy(CodingBlock block)
    {
        Destroy(block.gameObject);
    }
}

