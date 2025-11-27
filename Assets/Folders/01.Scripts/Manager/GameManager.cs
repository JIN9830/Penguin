using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    [field: SerializeField] public BlockCodingManager BlockCodingManager { get; private set; }
    [field: SerializeField] public CodingUIManager CodingUIManager { get; private set; }
    [field: SerializeField] public PlayerManager PlayerManager { get; private set; }
    [field: SerializeField] public StageManager StageManager { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterManager<T>(T manager) where T : MonoBehaviour
    {
        if (manager is PlayerManager playerManager)
        {
            PlayerManager = playerManager;
        }
        else if (manager is StageManager stageManager)
        {
            StageManager = stageManager;
        }
        else if (manager is BlockCodingManager blockCodingManager)
        {
            BlockCodingManager = blockCodingManager;
        }
    }
}
