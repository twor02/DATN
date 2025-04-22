using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UI_Ingame : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private PlayerInputSet playerinput;
    private List<Player> playerList;
    public static UI_Ingame instance;
    public UI_FadeEffect fadeEffect { get; private set; } // read-only

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;
    [SerializeField] private TextMeshProUGUI lifePointsText;


    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        playerinput = new PlayerInputSet();
    }

    private void OnEnable()
    {
        playerinput.Enable();

        playerinput.UI.Pause.performed += ctx => PauseButton();
        playerinput.UI.Navigate.performed += ctx => UpdateSelected();
    }
    private void OnDisable()
    {
        playerinput.Disable();

        playerinput.UI.Pause.performed -= ctx => PauseButton();
    }
    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);

        GameObject pressJoinText = FindFirstObjectByType<UI_TextBlinkEffect>().gameObject;
        PlayerManager.instance.objectsToDisable.Add(pressJoinText);
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits;
    }
    public void PauseButton()
    {
        playerList = PlayerManager.instance.GetPlayerList();

        if (isPaused)
            UnpauseTheGame();
        else
            PauseTheGame();
    }

    private void UpdateSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    private void PauseTheGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Disable();
        }

        EventSystem.current.SetSelectedGameObject(firstSelected);
        pauseUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
    }

    private void UnpauseTheGame()
    {
        foreach (var player in playerList)
        {
            player.playerInput.Enable();
        }

        pauseUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00") + " s";
    }
    public void UpdateLifePointsUI(int lifePoints, int maxLifePoints)
    {
        if(DifficultyManager.instance.difficulty == DifficultyType.Easy)
        {
            lifePointsText.transform.parent.gameObject.SetActive(false);
        }
        lifePointsText.text = lifePoints + "/" + maxLifePoints;
    }
}
