using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public int[] skinId;
    public static SkinManager instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetSkinId(int id, int playerNumber) => skinId[playerNumber] = id;
    public int GetSkinId(int playerNumber) => skinId[playerNumber];
}
