using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string groundLayerName = "Ground";
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void FlipSprite() => sr.flipX = !sr.flipX;
    public void SetVelocity(Vector2 velocity) => rb.linearVelocity = velocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            collision.GetComponent<Player>().KnockBack(transform.position.x);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLayerName))
        {
            Destroy(gameObject, .05f);
        }

    }
}
