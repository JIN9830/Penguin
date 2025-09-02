using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    public BlockCategory EBlockCategory { get; set; }

    public static ObjectPoolManager Instance { get; private set; }

    public Dictionary<BlockCategory, IObjectPool<CodingBlock>> PoolManagementDic { get; private set; } = new Dictionary<BlockCategory, IObjectPool<CodingBlock>>();

    public Dictionary<BlockCategory, GameObject> PoolObjectDic { get; private set; } = new Dictionary<BlockCategory, GameObject>();

    [Header("Object Pool 정보")]
    public ObjectInfo[] objectInfo;


    private void Awake()
    {
        #region Singleton Code
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
            Destroy(this.gameObject);
        #endregion


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

            PoolObjectDic.Add(objectInfo[index].objectName, objectInfo[index].prefab);
            PoolManagementDic.Add(objectInfo[index].objectName, pool);
        }
    }

    public CodingBlock SelectBlockFromPool(BlockCategory selectBlockName)
    {
        EBlockCategory = selectBlockName;

        return PoolManagementDic[selectBlockName].Get();
    }

    public CodingBlock CreateBlockObject()
    {
        CodingBlock newBlock = Instantiate(PoolObjectDic[EBlockCategory]).GetComponent<CodingBlock>();
        newBlock.Pool = PoolManagementDic[EBlockCategory];
        return newBlock;
    }
    public void OnBlockGet(CodingBlock block)
    {
        block.gameObject.SetActive(true);
    }
    public void OnBlockRelease(CodingBlock block)
    {
        // .. 오브젝트를 풀에 반환하기 전에 다른 오브젝트의 자식으로 이동합니다. (Grid Layout Group 내부의 오브젝트 순서가 섞이는것을 방지)
        block.transform.SetParent(CodingUIManager.Instance.ReleasedBlocks.transform);
        block.gameObject.SetActive(false);
    }
    public void OnBlockDestroy(CodingBlock block)
    {
        Destroy(block.gameObject);
    }
}