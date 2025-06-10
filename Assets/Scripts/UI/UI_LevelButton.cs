using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;
    public static UI_LevelButton instance;

    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI dMYText;
    [SerializeField] private TextMeshProUGUI fruitsText;

    private int levelIndex;
    public string sceneName;
    
    
    public void SetupButton(int newLevelIndex)
    {

        levelIndex = newLevelIndex;

        levelNumberText.text = "Level " + levelIndex;
        sceneName = "Level_" + levelIndex;

        bestTimeText.text = TimerInfoText();
        dMYText.text = DateCompletedText();
        fruitsText.text = FruitsInfoText();
    }

    public void LoadLevel()
    {
        AudioManager.instance.PlaySFX(5);
        int difficultyIndex = ((int)DifficultyManager.instance.difficulty);
        PlayerPrefs.SetInt("GameDifficulty", difficultyIndex);
        SceneManager.LoadScene(sceneName);
    }

    private string TimerInfoText()
    {
        float timerValue = PlayerPrefs.GetFloat("Level" + levelIndex + "BestTime", 99);

        return "Best Time: " + timerValue.ToString("00") + "s"; 
    }
    private string FruitsInfoText()
    {
        int totalFruits = PlayerPrefs.GetInt("Level" + levelIndex + "TotalFruits", 0);
        string totalFruitsText = totalFruits == 0 ? "?" : totalFruits.ToString();

        int fruitsCollected = PlayerPrefs.GetInt("Level" + levelIndex + "FruitsCollected");
        return "Fruits: " + fruitsCollected + " / " + totalFruitsText;
    }
    private string DateCompletedText()
    {
        string dateKey = "Level" + levelIndex + "DateCompleted";
        string savedDate = PlayerPrefs.GetString(dateKey, "Day: ??/??/????");
        return savedDate;
    }

}
