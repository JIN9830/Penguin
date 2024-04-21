using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;
using static GameManager;
using static ObjectPoolManager;

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

    public BlockCategory blockName;

    [Header("Pool 내부 오브젝트 정보")]
    public ObjectInfo[] objectInfo;

    public Dictionary<BlockCategory, IObjectPool<CodingBlock>> poolManagerDic = new Dictionary<BlockCategory, IObjectPool<CodingBlock>>();

    public Dictionary<BlockCategory, GameObject> poolObjectDic = new Dictionary<BlockCategory, GameObject>();
    public GameObject selectedPoolObject = null;

    private void Awake()
    {
        for (int index = 0; index < objectInfo.Length; index++)
        {
            IObjectPool<CodingBlock> pool = new ObjectPool<CodingBlock>(
            createFunc: CreateBlockObject,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnBlockRelease,
            actionOnDestroy: OnBlockDestroy,
            collectionCheck: false,
            defaultCapacity: objectInfo[index].poolCapacity,
            maxSize: objectInfo[index].poolCapacity
            );

            poolObjectDic.Add(objectInfo[index].objectName, objectInfo[index].prefab);
            poolManagerDic.Add(objectInfo[index].objectName, pool);

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

    public void SelectedPoolObject(BlockCategory selectedBlockName)
    {
        blockName = selectedBlockName;
        poolObjectDic.TryGetValue(selectedBlockName, out selectedPoolObject);
    }

    public CodingBlock CreateBlockObject()
    {
        CodingBlock newBlock = Instantiate(poolObjectDic[blockName]).GetComponent<CodingBlock>();
        newBlock.GetComponent<CodingBlock>().Pool = poolManagerDic[blockName];
        return newBlock;
    }

    public CodingBlock SelectBlockFromPool(BlockCategory block)
    {
        blockName = block;

        return poolManagerDic[block].Get();
    }
    public void OnTakeFromPool(CodingBlock block)
    {
        block.gameObject.SetActive(true);
    }
    public void OnBlockRelease(CodingBlock block)
    {
        // .. 오브젝트를 풀에 반환하기 전에 다른 오브젝트로 이동합니다.
        block.transform.SetParent(CodingUIManager_Instance.ReleasedBlocks.transform);
        block.gameObject.SetActive(false);
    }
    public void OnBlockDestroy(CodingBlock block)
    {
        Destroy(block.gameObject);
    }
}

