using UnityEngine;

public class Trap_Trampoline : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private float pushPower;
    [SerializeField] private float duration = .5f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if(player != null)
        {
            AudioManager.instance.PlaySFX(12);
            player.Push(transform.up * pushPower, duration);
            anim.SetTrigger("activate");
        }
    }
}
