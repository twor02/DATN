using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Credits : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    private bool creditSkipped;

    [SerializeField] private RectTransform rectT;
    [SerializeField] private float scrollSpeed = 200;
    [SerializeField] private float offScreenPosition = 1500;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        fadeEffect.ScreenFade(0, 1);
    }
    private void Update()
    {
        rectT.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        if(rectT.anchoredPosition.y > offScreenPosition)
        {
            GoToMainMenu();
        }
    }

    public void SkipCredits()
    {
        if(creditSkipped == false)
        {
            scrollSpeed *= 10;
            creditSkipped = true;
        }
        else
        {
            GoToMainMenu();
        }
    }
    private void GoToMainMenu() => fadeEffect.ScreenFade(1, 1, SwitchToMainMenu);
    private void SwitchToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
