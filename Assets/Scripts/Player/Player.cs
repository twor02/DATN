using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private DifficultyType gameDifficulty;
    [SerializeField] private GameObject fruitDrop;
    private GameManager gameManager;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    public InputActionAsset playerInput { get; private set; }
    private Vector2 moveInput;

    private bool canBeControlled = false;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;
    private float defaultGravityScale;

    [Header("Wall Interaction")]
    [SerializeField] private float wallJumpDuration = 0.6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = 0.25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = 0.5f;
    private float coyoteJumpActivated = -1;

    [Header("KnockBack")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;
    private Coroutine knockbackRoutine;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;

    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;


    private bool facingRight = true;
    private int facingDir = 1;

    [Header("Player visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private ParticleSystem dustFx;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();

        playerInput = GetComponent<PlayerInput>().actions;
    }
    void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.instance;

        UpdateGameDifficulty();
        RespawnFinished(false);
        //UpdateSkin();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirborneStatus();

        if (canBeControlled == false)
        {
            HandleCollision();
            HandleAnimation();
            return;
        }

        if (isKnocked) return;

        HandleEnemyDetection();
        //HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollision();
        HandleAnimation();
    }

    public void Damage()
    {
        if (gameDifficulty == DifficultyType.Normal)
        {
            if (gameManager.FruitsCollected() <= 0)
            {
                Die();
                //gameManager.RestartLevel();
            }
            else
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform, true);
                gameManager.RemoveFruit();
            }
            return;
        }
        if (gameDifficulty == DifficultyType.Hard)
        {
            Die();
            //gameManager.RestartLevel();
        }
    }

    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    public void UpdateSkin(int skinIndex)
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null)
        {
            return;
        }
        GetComponentInChildren<Animator>().runtimeAnimatorController = animators[skinIndex];
    }

    private void HandleEnemyDetection()
    {
        if (rb.linearVelocity.y >= 0) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemies newEnemy = enemy.GetComponent<Enemies>();
            if (enemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
            }
        }
    }

    public void RespawnFinished(bool finished)
    {
        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(11);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }

    //public void KnockBack(float sourceDamageXPosition)
    //{
    //    float knockbackDir = 1;
    //    if (transform.position.x < sourceDamageXPosition) knockbackDir = -1;
    //    if (isKnocked) return;

    //    AudioManager.instance.PlaySFX(9);
    //    CameraManager.instance.ScreenShake(knockbackDir);
    //    StartCoroutine(KnockBackRroutine());
    //    rb.linearVelocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    //}

    public void KnockBack(float sourceDamageXPosition)
    {
        if (isKnocked) return;

        isKnocked = true;

        float knockbackDir = (transform.position.x < sourceDamageXPosition) ? -1 : 1;

        AudioManager.instance.PlaySFX(9);
        CameraManager.instance.ScreenShake(knockbackDir);

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y), ForceMode2D.Impulse);

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockBackRoutine());
    }

    private IEnumerator KnockBackRoutine()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(0);

        GameObject newDeathVfx = Instantiate(deathVfx, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutine(direction, duration));
    }

    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);
        canBeControlled = true;
    }

    private void UpdateAirborneStatus()
    {
        if (!isGrounded && !isAirborne) BecomeAirborne();
        if (isGrounded && isAirborne) HandleLanding();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;
        if (rb.linearVelocity.y < 0) ActivateCoyoteJump();
    }
    private void HandleLanding()
    {
        dustFx.Play();
        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        //xInput = Input.GetAxisRaw("Horizontal");
        //yInput = Input.GetAxisRaw("Vertical");

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    JumpButton();
        //    RequestBufferJump();
        //}
    }

    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if (isAirborne) bufferJumpActivated = Time.time;
    }
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }
    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;
    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;
    #endregion
    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;
        if (isGrounded || coyoteJumpAvailable)
        {
            Jump();
        }
        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }

    private void Jump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); //rb.linearVelocityY = jumpForce;
    }

    private void DoubleJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);
        StopCoroutine(WallJumpRoutine());
        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }
    private void WallJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(12);
        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }
    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = moveInput.y < 0 ? 1 : 0.5f;

        if (canWallSlide == false)
        {
            return;
        }

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);
    }
    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(wallJumpDuration);
        isWallJumping = false;
    }
    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleMovement()
    {
        if (isWallDetected) return;
        if (isWallJumping) return;

        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        //rb.linearVelocityX = xInput * speed;
    }

    private void HandleAnimation()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }
    private void HandleFlip()
    {
        if (moveInput.x < 0 && facingRight || moveInput.x > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.FindAction("Jump").performed += OnJumpPerformed;
        playerInput.FindAction("Movement").performed += OnMovementPerformed;
        playerInput.FindAction("Movement").canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.FindAction("Jump").performed -= OnJumpPerformed;
        playerInput.FindAction("Movement").performed -= OnMovementPerformed;
        playerInput.FindAction("Movement").canceled -= OnMovementCanceled;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpButton();
        AttemptBufferJump();
    }
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
  

}
