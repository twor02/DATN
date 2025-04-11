using UnityEditor;
using UnityEngine;

public enum FruitType { Apple, Banana, Orange, Kiwi, Melon, Pineapple, Cherry, Strawberry};
public class Fruit : MonoBehaviour
{
    [SerializeField] private FruitType fruitType;
    [SerializeField] private GameObject pickupVfx;

    private GameManager gameManager;
    protected Animator anim;
    protected SpriteRenderer sr;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();   
        sr = GetComponentInChildren<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        gameManager = GameManager.instance;
        SetRandomLookIfNeeded();
    }
    
    private void SetRandomLookIfNeeded()
    {
        if ((gameManager.FruitsHaveRandomLook() == false)) 
        {
            UpdateFruitVisuals();
            return;
        }
        int randomIndex = Random.Range(0, 8);
        anim.SetFloat("fruitIndex", randomIndex);
    }

    private void UpdateFruitVisuals() => anim.SetFloat("fruitIndex", (int)fruitType);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            gameManager.AddFruit();
            AudioManager.instance.PlaySFX(8);
            Destroy(gameObject);

            GameObject newVfx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
            Destroy(newVfx, 0.5f);
        }
    }
}
