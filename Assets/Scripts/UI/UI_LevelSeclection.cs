using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSeclection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    private void Start()
    {
        CreateLevelButtons();
    }
    private void CreateLevelButtons()
    {
        int levelAmount = SceneManager.sceneCountInBuildSettings - 1;

        for(int i = 1;  i < levelAmount; i++)
        {
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.SetupButton(i);
        }
    }
}
