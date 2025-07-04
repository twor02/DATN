using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEditor;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lastSelected;
    private DefaultInputActions defaultInput;
    private UI_FadeEffect fadeEffect;
    public string firstLevelName;

    [SerializeField] private GameObject[] uiElements;

    [SerializeField] private GameObject continueButton;

    [Header("Interactive Camera")]
    [SerializeField] private MenuCharacter menuCharacter;
    [SerializeField] private CinemachineCamera cinemachine;
    [SerializeField] private Transform mainMenuPoint;
    [SerializeField] private Transform skinSelectionPoint;
    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();

        defaultInput = new DefaultInputActions();
    }

    private void Start()
    {
        //if (HasLevelProgression())
        //{
        //    continueButton.SetActive(true);
        //}
        fadeEffect.ScreenFade(0, 1.5f);
    }

    private void OnEnable()
    {
        defaultInput.Enable();
        defaultInput.UI.Navigate.performed += ctx => UpdateSelected();
    }

    private void OnDisable()
    {
        defaultInput.Disable();
        defaultInput.UI.Navigate.performed -= ctx => UpdateSelected();
    }

    public void UpdateLastSelected(GameObject newLastSelected)
    {
        lastSelected = newLastSelected;
    }
    private void UpdateSelected()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
            EventSystem.current.SetSelectedGameObject(lastSelected);
    }
    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (GameObject ui in uiElements)
        {
            ui.SetActive(false);
        }
        uiToEnable.SetActive(true);

        AudioManager.instance.PlaySFX(4);
    }
    public void NewGame()
    {
        fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene);
        AudioManager.instance.PlaySFX(4);
    }
    private void LoadLevelScene() => SceneManager.LoadScene(firstLevelName);

    private bool HasLevelProgression()
    {
        bool hasLevelProgression = PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;
        return hasLevelProgression;
    }
    public void CoutinueGame()
    {
        int levelToLoad = PlayerPrefs.GetInt("ContinueLevelNumber", 0);
        int difficultyIndex = PlayerPrefs.GetInt("GameDifficulty", 1);
        int lastSavedSkin = PlayerPrefs.GetInt("lastUsedSkin");

        //SkinManager.instance.SetSkinId(lastSavedSkin);
        DifficultyManager.instance.LoadDifficulty(difficultyIndex);
        SceneManager.LoadScene("Level_" + levelToLoad);
        AudioManager.instance.PlaySFX(5);
    }
    public void MoveCameraToMainMenu()
    {
        menuCharacter.MoveTo(mainMenuPoint);
        cinemachine.Follow = mainMenuPoint;
    }
    public void MoveCameraToSkinMenu()
    {
        menuCharacter.MoveTo(skinSelectionPoint);
        cinemachine.Follow = skinSelectionPoint;
    }

    public  void QuitButton()
    {
        //if(EditorApplication.isPlaying)
        //    EditorApplication.isPlaying = false;
        //else
            Application.Quit();
    }
}
