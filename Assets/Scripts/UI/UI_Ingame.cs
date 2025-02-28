using UnityEngine;

public class UI_Ingame : MonoBehaviour
{
    public static UI_Ingame instance;
    public UI_FadeEffect fadeEffect;

    private void Awake()
    {
        instance = this;

        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }
    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }
}
