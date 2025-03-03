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
        nextLevelIndex = currentLevelIndex + 1;

        CollecteFruitsInfo();
    }

    private void Update()
    {
        levelTimer += Time.deltaTime;

        inGameUI.UpdateTimerUI(levelTimer);
    }

    private void CollecteFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;

        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits);
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    public void ReSpawnPlayer() => StartCoroutine(RespawnCoroutine());
    
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
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);
    }
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
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
        }
    }
    private bool NomoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; //we have MainMenu Scene and TheEnd Scene. So we -2
        bool noMoreLevels = currentLevelIndex == lastLevelIndex;

        return noMoreLevels;
    }
}
