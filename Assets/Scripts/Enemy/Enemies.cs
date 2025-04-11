using UnityEngine;
using UnityEngine.Windows;

public class Enemies : MonoBehaviour
{
    private SpriteRenderer sr => GetComponent<SpriteRenderer>();
    protected Animator anim;
    protected Rigidbody2D rb;
    protected Collider2D[] colliders;
    protected Transform player;
    

    [Header("General info")]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float idleDuration = 1.5f;
    protected bool canMove = true;
    protected float idleTimer;

    [Header("Death details")]
    [SerializeField] protected float deathImpactSpeed = 5;
    [SerializeField] protected float deathRotationSpeed = 150;
    protected int deathRotationDirection = 1;
    protected bool isDead;
 
    [Header("Basic Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected float playerDectectionDistance = 7;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected Transform groundCheck;
    protected bool isPlayerDetected;
    protected bool isGrounded;
    protected bool isWallDetected;
    protected bool isGroundInfrontDetected;

    protected int facingDir = -1;
    protected bool facingRight = false;


    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        if(sr.flipX == true && !facingRight)
        {
            sr.flipX = false;
            Flip();
        }

        PlayerManager.OnPlayerRespawn += UpdatePlayersRef;
    }

    private void UpdatePlayersRef()
    {
        if (player == null)
            player = PlayerManager.instance.player.transform;
    }

    protected virtual void Update()
    {
        HandleCollision();
        HandleAnimator();

        idleTimer -= Time.deltaTime;

        if (isDead)
        {
            HandleDeathRotation();
        }
    }

    public virtual void Die()
    {
        foreach(var collider in colliders)
        {
            collider.enabled = false;
        }
        anim.SetTrigger("isHit");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpactSpeed);
        isDead = true;

        if(Random.Range(0,100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1;
        }

        PlayerManager.OnPlayerRespawn -= UpdatePlayersRef;
        Destroy(gameObject, 10);
    }
    private void HandleDeathRotation()
    {
        transform.Rotate(0,0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    protected virtual void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    [ContextMenu("Change Facing Direction")]
    protected virtual void FlipDefaultFacingDirection()
    {
        sr.flipX = !sr.flipX;
    }

    protected virtual void HandleAnimator()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInfrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
        isPlayerDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, playerDectectionDistance, whatIsPlayer);

    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (playerDectectionDistance * facingDir), transform.position.y));
    }
}
