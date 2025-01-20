using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruit Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;

    [Header("Checkpoints")]
    public bool canReactivate;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CollecteFruitsInfo();
    }

    private void CollecteFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;
    }

    public void UpdateRespawnPoint(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;
    public void ReSpawnPlayer() => StartCoroutine(RespawnCoroutine());
    
    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        //player = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity).GetComponent<Player>();
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
    }
    public void AddFruit()  => fruitsCollected++;
    public bool FruitsHaveRandomLook() => fruitsAreRandom;
}
