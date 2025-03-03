using TMPro;
using UnityEngine;

public class UI_Ingame : MonoBehaviour
{
    public static UI_Ingame instance;
    public UI_FadeEffect fadeEffect { get; private set; } // read-only

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }
    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }

    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits;
    }
    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00") + " s";
    }
}
