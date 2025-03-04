using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Ingame : MonoBehaviour
{
    public static UI_Ingame instance;
    public UI_FadeEffect fadeEffect { get; private set; } // read-only

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }
    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            PauseButton();
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits;
    }
    public void PauseButton()
    {
        if (isPaused)
        {
            pauseUI.SetActive(false);
            isPaused = false;
            Time.timeScale = 1;
        }
        else
        {
            pauseUI.SetActive(true);
            isPaused = true;
            Time.timeScale = 0;
        }
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00") + " s";
    }
}
