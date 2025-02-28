using UnityEngine;

public class Enemy_Chicken : Enemies
{
    [Header("Chicken details")]
    [SerializeField] private float aggroDuration;

    private float aggroTimer;
    private bool canFlip = true;
    protected override void Update()
    {
        base.Update();

        aggroTimer -= Time.deltaTime;

        if (isDead) return;

        if (isPlayerDetected)
        {
            canMove = true;
            aggroTimer = aggroDuration;
        }

        if(aggroTimer <= 0)
        {
            canMove = false;
        }

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    private void HandleTurnAround()
    {
        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            canMove = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if (canMove == false) return;

        HandleFlip(player.transform.position.x);
        //kiem soat toc do chicken
        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }

    protected override void HandleFlip(float xValue)
    {
        if (xValue < transform.position.x && facingRight || xValue > transform.position.x && !facingRight)
        {
            if (canFlip)
            {
                //dat canFlip ve false de chi flip 1 lan
                canFlip = false;
                Invoke(nameof(Flip), .2f);
            }
        }
    }

    protected override void Flip()
    {
        base.Flip();
        canFlip = true;
    }

   
}
