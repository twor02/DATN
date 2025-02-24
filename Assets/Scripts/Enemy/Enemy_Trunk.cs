using UnityEngine;

public class Enemy_Trunk : Enemies
{
    [Header("Trunk details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastTimeAttacked;

    protected override void Update()
    {
        base.Update();


        if (isDead) return;

        //it only can attack agian if the current time in the game > lastTimeAttacked + cooldown 
        //if current time in game is 7 > (5 + 1.5), now it can attack again
        bool canAttack = Time.time > lastTimeAttacked + attackCooldown;

        if (isPlayerDetected && canAttack)
            Attack();

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }
    private void Attack()
    {
        idleTimer = idleDuration + attackCooldown;
        lastTimeAttacked = Time.time;
        anim.SetTrigger("isAttack");
    }
    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        Vector2 bulletVelocity = new Vector2(facingDir * bulletSpeed, 0);
        newBullet.SetVelocity(bulletVelocity);

        if(facingDir == 1)
        {
            newBullet.FlipSprite();
        }

        Destroy(newBullet.gameObject, 10);
    }

    private void HandleTurnAround()
    {
        if (!isGroundInFrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }
    private void HandleMovement()
    {
        if (idleTimer > 0) return;

        rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }  
}
