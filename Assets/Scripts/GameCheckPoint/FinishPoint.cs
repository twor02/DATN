using UnityEngine;

public class FinishPoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private int playerInTrigger;

    private bool CanFinishLevel()
    {
        if(playerInTrigger == PlayerManager.instance.playerCountWinCondition)
            return true;
        return false;
    }
    private bool CompareFruit()
    {
        if(GameManager.instance.fruitsCollected == GameManager.instance.totalFruits) 
            return true;
        return false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            playerInTrigger++;
            AudioManager.instance.PlaySFX(2);
            anim.SetTrigger("activate");

            if(CanFinishLevel() && CompareFruit())
            {
                GameManager.instance.LevelFinished();
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            playerInTrigger--;
        }
    }
}
