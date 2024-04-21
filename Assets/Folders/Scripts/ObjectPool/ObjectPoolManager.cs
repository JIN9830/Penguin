using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using static GameManager;

public class ObjectPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class ObjectInfo
    {
        public BlockCategory objectName;
        public GameObject prefab;
        public int poolCapacity;
    }

    public enum BlockCategory
    {
        Forward,
        Left,
        Right,
        Function,
        Loop,
    } 
    
    [Header("Pool 내부 오브젝트 정보")]
    public ObjectInfo[] objectInfo;

    public BlockCategory BlockName { get; set; }

    public Dictionary<BlockCategory, IObjectPool<CodingBlock>> poolManagedDic = new Dictionary<BlockCategory, IObjectPool<CodingBlock>>();

    public Dictionary<BlockCategory, GameObject> poolObjectDic = new Dictionary<BlockCategory, GameObject>();

    private void Awake()
    {
        for (int index = 0; index < objectInfo.Length; index++)
        {
            IObjectPool<CodingBlock> pool = new ObjectPool<CodingBlock>(
            createFunc: CreateBlockObject,
            actionOnGet: OnBlockGet,
            actionOnRelease: OnBlockRelease,
            actionOnDestroy: OnBlockDestroy,
            collectionCheck: false,
            defaultCapacity: objectInfo[index].poolCapacity,
            maxSize: objectInfo[index].poolCapacity
            );

            poolObjectDic.Add(objectInfo[index].objectName, objectInfo[index].prefab);
            poolManagedDic.Add(objectInfo[index].objectName, pool);

            //for (int i = 0; i < objectInfo[index].poolCapacity; i++)
            //{
            //    CodingBlock poolCodingBlock = CreateBlockObject();
            //    poolCodingBlock.ReleaseBlock();
            //}
        }
    }

    private void Start()
    {
        GameManager_Instance.Get_ObjectPoolManager(this.gameObject);
    }

    public CodingBlock SelectBlockFromPool(BlockCategory selectBlockName)
    {
        BlockName = selectBlockName;

        return poolManagedDic[selectBlockName].Get();
    }

    public CodingBlock CreateBlockObject()
    {
        CodingBlock newBlock = Instantiate(poolObjectDic[BlockName]).GetComponent<CodingBlock>();
        newBlock.GetComponent<CodingBlock>().Pool = poolManagedDic[BlockName];
        return newBlock;
    }
    public void OnBlockGet(CodingBlock block)
    {
        block.gameObject.SetActive(true);
    }
    public void OnBlockRelease(CodingBlock block)
    {
        // .. 오브젝트를 풀에 반환하기 전에 다른 오브젝트의 자식으로 이동합니다. (Layout 내부의 오브젝트 순서가 섞이는것을 방지)
        block.transform.SetParent(CodingUIManager_Instance.ReleasedBlocks.transform);
        block.gameObject.SetActive(false);
    }
    public void OnBlockDestroy(CodingBlock block)
    {
        Destroy(block.gameObject);
    }
}

