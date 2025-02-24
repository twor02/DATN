using UnityEngine;

public class EnemySnail_Body : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private float zRotation;

    public void SetupBody(float yVelocity, float zRotation, int facingDir)
    {
        rb = GetComponent<Rigidbody2D>();
        sr = rb.GetComponent<SpriteRenderer>();

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, yVelocity);

        this.zRotation = zRotation;

        if(facingDir == 1)
        {
            sr.flipX = true;
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }
}
