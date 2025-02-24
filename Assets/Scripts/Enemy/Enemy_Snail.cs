using UnityEngine;

public class Enemy_Snail : Enemies
{
    [Header("Snail details")]
    [SerializeField] private EnemySnail_Body bodyPrefab;
    [SerializeField] private float maxSpeed = 10;
    private bool hasBody = true;

    protected override void Update()
    {
        base.Update();


        if (isDead) return;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    public override void Die()
    {
        if (hasBody)
        {
            canMove = false;
            hasBody = false;
            anim.SetTrigger("isHit");

            rb.linearVelocity = Vector2.zero;
            idleDuration = 0;
        }
        else if(canMove == false && hasBody == false) 
        {
            anim.SetTrigger("isHit");
            canMove = true;
            moveSpeed = maxSpeed;
        }
        else
        {
            base.Die();
        }
    }

    private void HandleTurnAround()
    {
        bool canFlipFromLedge = !isGroundInFrontDetected && hasBody;
        if (canFlipFromLedge || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }
    private void HandleMovement()
    {
        if (idleTimer > 0) return;

        if(canMove == false) return;

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }
    private void CreateBody()
    {
        EnemySnail_Body  newBody = Instantiate(bodyPrefab, transform.position, Quaternion.identity);

        if (Random.Range(0, 100) < 50)
        {
            deathRotationDirection = deathRotationDirection * -1;
        }

        newBody.SetupBody(deathImpactSpeed, deathRotationSpeed * deathRotationDirection, facingDir);
        Destroy(newBody.gameObject, 10);
    }
    protected override void Flip()
    {
        base.Flip();

        if (hasBody == false) anim.SetTrigger("isWallHit");
    }
}
