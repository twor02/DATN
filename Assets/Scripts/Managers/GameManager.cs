using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UI_Ingame inGameUI;

    [Header("Level Managment")]
    [SerializeField] private float levelTimer;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruit Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;

    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Traps")]
    public GameObject arrowPrefeb;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        inGameUI = UI_Ingame.instance;

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        if(respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<StartPoint>().transform;
        }
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        nextLevelIndex = currentLevelIndex + 1;

        CollecteFruitsInfo();
        CreateManagersIfNeeded(); 
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        inGameUI.UpdateTimerUI(levelTimer);
    }
    private void CreateManagersIfNeeded()
    {
        if(AudioManager.instance == null)
            Instantiate(audioManager);
    }

    private void CollecteFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;

        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits);
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    public void ReSpawnPlayer()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
            return;

        StartCoroutine(RespawnCoroutine());
    }
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        //player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
    }
    public void AddFruit()
    {
        fruitsCollected++;
        inGameUI.UpdateFruitUI(fruitsCollected,totalFruits);
    }

    public void RemoveFruit()
    {
        fruitsCollected--;
        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);
    }
    public int FruitsCollected() => fruitsCollected;
    public bool FruitsHaveRandomLook() => fruitsAreRandom;

    public void CreateObject(GameObject prefab, Transform target, float delay = 0)
    {
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }

    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay)
    {
        Vector3 newPosition = target.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);   
    }
    public void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        SaveFruitsInfo();

        LoadNextScene();
    }
    private void SaveFruitsInfo()
    {
        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected");

        if(fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected);

        int totalFruitsBank = PlayerPrefs.GetInt("TotalFruitsAmount");
        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsBank + fruitsCollected);
    }

    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 99);
        if (levelTimer < lastTime)
            PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer);
    }
    private void SaveLevelProgression()
    {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);
        if (NomoreLevels() == false)
        {
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);

            //SkinManager skinManager = SkinManager.instance;
            //if (skinManager != null)
                PlayerPrefs.SetInt("lastUsedSkin", SkinManager.instance.GetSkinId());
        }
            
    }
    public void RestartLevel()
    {
        UI_Ingame.instance.fadeEffect.ScreenFade(1, .75f, LoadCurrentScene);
    }
    private void LoadCurrentScene() => SceneManager.LoadScene("Level_" + currentLevelIndex);
    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");
    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }
    public void LoadNextScene()
    {
        UI_FadeEffect fadeEffect = UI_Ingame.instance.fadeEffect;

        if (NomoreLevels())
        {
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        }
        else
        {
            fadeEffect.ScreenFade(1, .75f, LoadNextLevel);
        }
    }
    private bool NomoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; //we have MainMenu Scene and TheEnd Scene. So we -2
        bool noMoreLevels = currentLevelIndex == lastLevelIndex;

        return noMoreLevels;
    }
}
