using System;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private bool active;

    [SerializeField] private bool canBeReactivated;

    private void Start()
    {
        canBeReactivated = GameManager.instance.canReactivate;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(active && canBeReactivated == false) return;

        Player player = collision.GetComponent<Player>();

        if ((player != null)) ActivateCheckpoint();
        
    }

    private void ActivateCheckpoint()
    {
        active = true;
        anim.SetTrigger("activate");
        PlayerManager.instance.UpdateRespawnPoint(transform);
    }
}
