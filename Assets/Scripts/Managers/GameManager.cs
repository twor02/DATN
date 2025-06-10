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

    [Header("Fruit Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;
    public Transform fruitParent;

    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private SkinManager skinManager;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private ObjectCreator objectCreator;

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
        CreateManagersIfNeeded(); 

        PlayerManager.instance.EnableJoinAndUpdateLifePoints();
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
        if(PlayerManager.instance == null)
            Instantiate(playerManager);
        if(SkinManager.instance == null)
            Instantiate(skinManager);
        if (DifficultyManager.instance == null)
            Instantiate(difficultyManager);
        if(ObjectCreator.instance == null)
            Instantiate(objectCreator);
    }

    private void CollecteFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;

        inGameUI.UpdateFruitUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits);
    }
    [ContextMenu("Parent All Fruits")]
    private void ParentAllTheFruit()
    {
        if (fruitParent == null)
             return;

        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);

        foreach(Fruit fruit in allFruits)
        {
            fruit.transform.parent = fruitParent;
        }
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

   
    public void LevelFinished()
    {

        string dateKey = "Level" + currentLevelIndex + "DateCompleted";
        string currentDate = System.DateTime.Now.ToString("dd/MM/yyyy");
        PlayerPrefs.SetString(dateKey, currentDate);
        PlayerPrefs.Save();
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
                PlayerPrefs.SetInt("lastUsedSkin", SkinManager.instance.GetSkinId(0));
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
