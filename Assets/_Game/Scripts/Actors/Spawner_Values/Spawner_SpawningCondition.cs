using UnityEngine;

public class Spawner_SpawningCondition : MonoBehaviour
{
    private bool m_isInCooldown; //flag to manage the spawning speed
    private bool m_isPlayerTouchingScreen; //flag to manage spawning  with the input controls
    private bool m_canSpawnInLevel; //flag to manage the overall state of spawning

    private float m_cooldownTimer;
    private float m_cooldownDuration;
    
    public bool CanSpawn
    {
        get => (!m_isInCooldown && m_isPlayerTouchingScreen && m_canSpawnInLevel);
    }
    
    private void OnEnable()
    {
        Controller.OnTapBegin += StartSpawning;
        Controller.OnRelease += StopSpawning;

        Controller_Level.OnLevelStart += EnableSpawnInLevel;
        Controller_Level.OnFinishedLevel += DisableSpawnInLevel;
        
        Controller_LevelSection.OnNextLevelSectionLoaded += EnableSpawnInLevel;
        Controller_LevelSection.OnStartLoadingNextSectionLevel += DisableSpawnInLevel;
        Controller_LevelSection.OnFinishedLevel += DisableSpawnInLevel;
    }

    private void OnDisable()
    {
        Controller.OnTapBegin -= StartSpawning;
        Controller.OnRelease -= StopSpawning;

        Controller_Level.OnLevelStart -= EnableSpawnInLevel;
        Controller_Level.OnFinishedLevel -= DisableSpawnInLevel;
        
        Controller_LevelSection.OnNextLevelSectionLoaded -= EnableSpawnInLevel;
        Controller_LevelSection.OnStartLoadingNextSectionLevel -= DisableSpawnInLevel;
        Controller_LevelSection.OnFinishedLevel -= DisableSpawnInLevel;
    }

    private void Update()
    {
        ManageCooldown();
    }

    
    private void ManageCooldown()
    {
        if(!m_isInCooldown)
            return;

        m_cooldownTimer += Time.deltaTime;

        if (m_cooldownTimer >= m_cooldownDuration)
            m_isInCooldown = false;
    }


    public void EnterCooldown(float cooldownDuration)
    {
        m_isInCooldown = true;
        m_cooldownTimer = 0f;
        m_cooldownDuration = cooldownDuration;
    }

    private void EnableSpawnInLevel()
    {
        m_canSpawnInLevel = true;
    }

    private void DisableSpawnInLevel()
    {
        m_canSpawnInLevel = false;
    }

    private void StartSpawning(Vector3 cursorPosition)
    {
        m_isPlayerTouchingScreen = true;
    }

    private void StopSpawning(Vector3 cursorPosition)
    {
        m_isPlayerTouchingScreen = false;
    }
}
