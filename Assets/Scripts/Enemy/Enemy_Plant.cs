using UnityEngine;

public class Enemy_Plant : Enemies
{
    [Header("Plant details")]
    [SerializeField] private Enemy_Bullet bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastTimeAttacked;
    protected override void Update()
    {
        base.Update();

        //it only can attack agian if the current time in the game > lastTimeAttacked + cooldown 
        //if current time in game is 7 > (5 + 1.5), now it can attack again
        bool canAttack = Time.time > lastTimeAttacked + attackCooldown;

        if (isPlayerDetected && canAttack)
            Attack();
    }

    private void Attack()
    {
        lastTimeAttacked = Time.time;
        anim.SetTrigger("isAttack");
    }

    private void CreateBullet()
    {
        Enemy_Bullet newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        Vector2 bulletVelocity = new Vector2(facingDir * bulletSpeed, 0);
        newBullet.SetVelocity(bulletVelocity);

        Destroy(newBullet.gameObject, 10);
    }

    protected override void HandleAnimator()
    {
        //keep it empty, unless you need to update parameter
    }
}
