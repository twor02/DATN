using UnityEngine;
using UnityEngine.Windows;

public class Enemies : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    protected int facingDir = -1;
    protected bool facingRight = false;

    [SerializeField] protected GameObject damamgeTrigger;
    [Space]
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float idleDuration = 1.5f;
    protected float idleTimer;
    [Header("Death details")]
    [SerializeField] private float deathImpact = 5;
    [SerializeField] private float deathRotationSpeed = 150;
    private int deathRotationDirection = 1;
    protected bool isDead;
 
    [Header("Basic Collision")]
    [SerializeField] protected float groundCheckDistance = 1.1f;
    [SerializeField] protected float wallCheckDistance = .7f;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected Transform groundCheck;
    protected bool isGrounded;
    protected bool isWallDetected;
    protected bool isGroundInFrontDetected;


    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        idleTimer -= Time.deltaTime;

        if (isDead)
        {
            HandleDeathRotation();
        }
    }

    public virtual void Die()
    {
        damamgeTrigger.SetActive(false);
        anim.SetTrigger("isHit");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, deathImpact);
        isDead = true;

        if(Random.Range(0,100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1;
        }
    }
    private void HandleDeathRotation()
    {
        transform.Rotate(0,0, (deathRotationSpeed * deathRotationDirection) * Time.deltaTime);
    }

    protected virtual void HandleFlip(float xValue)
    {
        if (xValue < 0 && facingRight || xValue > 0 && !facingRight)
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

    protected virtual void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isGroundInFrontDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
