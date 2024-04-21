using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using static GameManager;

public class ObjectPoolManager : MonoBehaviour
{
    #region 오브젝트 풀 프리팹
    public enum BlockCategory
    {
        Forward,
        Left,
        Right,
        Function,
        Loop,
    }

    [System.Serializable]
    public class ObjectInfo
    {
        public BlockCategory objectName;
        public GameObject prefab;
    }

    [Header("Pool 내부 오브젝트 정보")]
    public ObjectInfo[] objectInfo;
    #endregion

    public Dictionary<BlockCategory, GameObject> PoolDictionary = new Dictionary<BlockCategory, GameObject>();
    public GameObject selectedPoolObject = null;

    public IObjectPool<CodingBlock> CodingBlockPool { get; private set; }

    [Header("Pool 크기 정보")]
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

        PoolDictionary.Add(BlockCategory.Forward, objectInfo[0].prefab);
        PoolDictionary.Add(BlockCategory.Left, objectInfo[1].prefab);
        PoolDictionary.Add(BlockCategory.Right, objectInfo[2].prefab);
        PoolDictionary.Add(BlockCategory.Function, objectInfo[3].prefab);
        PoolDictionary.Add(BlockCategory.Loop, objectInfo[4].prefab);

    }

    private void Start()
    {
        GameManager_Instance.Get_ObjectPoolManager(this.gameObject);
    }

    public void SelectedPoolObject(BlockCategory blockName)
    {
        PoolDictionary.TryGetValue(blockName, out selectedPoolObject);
    }

    public CodingBlock CreateBlockObject()
    {
        CodingBlock newBlock = Instantiate(selectedPoolObject).GetComponent<CodingBlock>();
        newBlock.GetComponent<CodingBlock>().Pool = this.CodingBlockPool;
        return newBlock;
    }

    public void OnBlockTakeFromPool(CodingBlock block)
    {
        block.gameObject.SetActive(true);
    }
    public void OnBlockRelease(CodingBlock block)
    {
        // .. 오브젝트를 풀에 반환하기 전에 다른 오브젝트로 이동합니다. (레이아웃 내부의 오브젝트 순서 때문에 하는 작업)
        block.transform.SetParent(CodingUIManager_Instance.ReleasedBlocks.transform);
        block.gameObject.SetActive(false);
    }
    public void OnBlockDestroy(CodingBlock block)
    {
        Destroy(block.gameObject);
    }
}

