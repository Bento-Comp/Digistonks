using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Manager_Level : MonoBehaviour
{
    public static Action<int> OnLoadLevel;
    public static Action<int> OnSendTotalLevelCleared;

    [SerializeField] private List<GameObject> m_levelPrefabList = null;

    [SerializeField] private Transform m_levelParent = null;

    [SerializeField] private string m_currentLevelIndexSaveTag = "CurrentLevelIndex";
    [SerializeField] private string m_totalLevelClearedSaveKey = "LevelCleared";

    private GameObject m_currentLevel;
    private int m_currentLevelIndex;
    private int m_totalLevelCleared;


    private void OnEnable()
    {
        Manager_GameState.OnBroadcastGameState += OnBroadcastGameState;
        Button_LoadLevel.OnLoadLevel_ButtonPressed += OnLoadLevel_ButtonPressed;
    }

    private void OnDisable()
    {
        Manager_GameState.OnBroadcastGameState -= OnBroadcastGameState;
        Button_LoadLevel.OnLoadLevel_ButtonPressed -= OnLoadLevel_ButtonPressed;
    }

    private void Awake()
    {
        LoadCurrentLevelIndex();
    }

    private void OnLoadLevel_ButtonPressed(LoadLevelType loadLevelType)
    {
        switch (loadLevelType)
        {
            case LoadLevelType.Reload:
                LoadCurrentLevel();
                break;
            case LoadLevelType.LoadNext:
                m_currentLevelIndex++;
                m_totalLevelCleared++;
                CheckLevelIndexValidity();
                LoadCurrentLevel();
                break;
            case LoadLevelType.LoadPrevious:
                m_currentLevelIndex--;
                CheckLevelIndexValidity();
                LoadCurrentLevel();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(loadLevelType), loadLevelType, null);
        }
    }

    private void OnBroadcastGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                LoadCurrentLevel();
                break;
            case GameState.InGame:
                break;
            case GameState.Gameover:
                Debug.Log("YSO : gameover");
                YsoCorp.GameUtils.YCManager.instance.OnGameFinished(false);
                break;
            case GameState.Victory:
                m_currentLevelIndex++;
                m_totalLevelCleared++;
                
                Debug.Log("YSO : victory");
                YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
                CheckLevelIndexValidity();

                SaveCurrentLevelIndex();
                SaveTotalLevelCleared();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    private void CheckLevelIndexValidity()
    {
        if (m_currentLevelIndex >= m_levelPrefabList.Count)
            m_currentLevelIndex = 0;

        if (m_currentLevelIndex < 0)
            m_currentLevelIndex = m_levelPrefabList.Count - 1;
    }

    private void LoadCurrentLevel()
    {
        if (m_currentLevel != null)
            Destroy(m_currentLevel);

        m_currentLevel = Instantiate(m_levelPrefabList[m_currentLevelIndex], m_levelParent);
        
        // YSO SDK
        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(m_totalLevelCleared);
        
        OnLoadLevel?.Invoke(m_currentLevelIndex);
    }

    private void LoadTotalLevelCleared()
    {
        if (PlayerPrefs.HasKey(m_totalLevelClearedSaveKey))
        {
            m_totalLevelCleared = PlayerPrefs.GetInt(m_totalLevelClearedSaveKey);
        }
        else
        {
            m_totalLevelCleared = 0;
            SaveTotalLevelCleared();
        }

        OnSendTotalLevelCleared?.Invoke(m_totalLevelCleared);
    }
    
    private void SaveTotalLevelCleared()
    {
        PlayerPrefs.SetInt(m_totalLevelClearedSaveKey, m_totalLevelCleared);
    }

    private void LoadCurrentLevelIndex()
    {
        if (PlayerPrefs.HasKey(m_currentLevelIndexSaveTag))
        {
            m_currentLevelIndex = PlayerPrefs.GetInt(m_currentLevelIndexSaveTag);

            m_currentLevelIndex = Mathf.Clamp(m_currentLevelIndex, 0, m_levelPrefabList.Count - 1);
        }
        else
        {
            m_currentLevelIndex = 0;
            SaveCurrentLevelIndex();
        }
    }

    private void SaveCurrentLevelIndex()
    {
        PlayerPrefs.SetInt(m_currentLevelIndexSaveTag, m_currentLevelIndex);
    }
}