using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.Damage();
            player.Die();
        }

        Enemies enemy = collision.gameObject.GetComponent<Enemies>();

        if(enemy != null)
        {
            enemy.Die();
        }
    }
}
