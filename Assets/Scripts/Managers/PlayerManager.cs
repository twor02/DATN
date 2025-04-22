using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action OnPlayerRespawn;
    public static event Action OnPlayerDeath;

    public PlayerInputManager playerInputManager {  get; private set; }
    public static PlayerManager instance;

    public List<GameObject> objectsToDisable;

    public int lifePoints;
    public int maxPlayerCount = 1;
    public int playerCountWinCondition;

    [Header("Player")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private string[] playerDevice;
    //[SerializeField] private GameObject playerPrefab;
    //[SerializeField] private float respawnDelay;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        playerInputManager = GetComponent<PlayerInputManager>();
        
    }

    private void PlaceNewPlayerAtRespawnPoint(Transform newPlayer)
    {
        if (respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<StartPoint>().transform;
        }

        newPlayer.position = respawnPoint.position;
    }

    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += AddPlayer;
        playerInputManager.onPlayerLeft += RemovePlayer;

        foreach (var gameObject in objectsToDisable)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        playerInputManager.onPlayerJoined -= AddPlayer;
        playerInputManager.onPlayerLeft -= RemovePlayer;
    }
    public void SetupMaxPlayersCount(int newPlayersCount)
    {
        maxPlayerCount = newPlayersCount;
        playerInputManager.SetMaximumPlayerCount(maxPlayerCount);
    }
    public void EnableJoinAndUpdateLifePoints()
    {
        playerInputManager.EnableJoining();
        playerCountWinCondition = maxPlayerCount;
        lifePoints = maxPlayerCount;
        UI_Ingame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);
    }

    private void AddPlayer(PlayerInput newPlayer)
    {
        Player playerScript = newPlayer.GetComponent<Player>();

        playerList.Add(playerScript);

        OnPlayerRespawn?.Invoke();
        PlaceNewPlayerAtRespawnPoint(newPlayer.transform);

        int newPLayerNumber = GetPlayerNumber(newPlayer);
        int newPlayerSkinId = SkinManager.instance.GetSkinId(newPLayerNumber);

        playerScript.UpdateSkin(newPlayerSkinId);
        
        foreach (var gameObject in objectsToDisable)
        {
            gameObject.SetActive(false);
        }
    }

    private int GetPlayerNumber(PlayerInput newPlayer)
    {
        int newPlayerNumber = 0;

        foreach (var device in newPlayer.devices)
        {
            for (int i = 0; i < playerDevice.Length; i++)
            {
                if (playerDevice[i] == "Empty")
                {
                    newPlayerNumber = i;
                    playerDevice[i] = device.name;
                    break;
                }
                else if (playerDevice[i] == device.name)
                {
                    newPlayerNumber = i;
                    break;
                }
            }
        }
        return newPlayerNumber;
    }

    private void RemovePlayer(PlayerInput player)
    {
        Player playerScript = player.GetComponent<Player>();
        playerList.Remove(playerScript);

        if(CanRemoveLifePoints() && lifePoints > 0)
            lifePoints--;

        if(lifePoints <= 0)
        {
            playerCountWinCondition--;
            playerInputManager.DisableJoining();

            if(playerList.Count <= 0)
                GameManager.instance.RestartLevel();
        }
        UI_Ingame.instance.UpdateLifePointsUI(lifePoints, maxPlayerCount);

        OnPlayerDeath?.Invoke();
    }

    private bool CanRemoveLifePoints()
    {
        if(DifficultyManager.instance.difficulty == DifficultyType.Hard)
        {
            return true;
        }
        if(GameManager.instance.fruitsCollected <= 0 && DifficultyManager.instance.difficulty == DifficultyType.Normal)
        {
            return true;
        }

        return false;
    }
    public void UpdateRespawnPoint(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    public List<Player> GetPlayerList() => playerList;
    //public void ReSpawnPlayer()
    //{
    //    DifficultyManager difficultyManager = DifficultyManager.instance;

    //    if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
    //        return;

    //    StartCoroutine(RespawnCoroutine());
    //}
    //private IEnumerator RespawnCoroutine()
    //{
    //    yield return new WaitForSeconds(respawnDelay);

    //    //player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
    //    GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
    //    player = newPlayer.GetComponent<Player>();
    //}
}
