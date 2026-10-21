using UnityEngine;

public class CloudPlatform : MonoBehaviour
{
    [Header("Behavior Settings")]
    public float stopYPosition = -4.8f;
    public float moveSpeed = 8f;
    public float lifetime = 15f;

    private Rigidbody2D rb;
    private PlayerControl playerController;
    private bool hasStopped = false;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (!hasStopped && transform.position.y <= stopYPosition)
        {
            hasStopped = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            transform.position = new Vector3(transform.position.x, stopYPosition, transform.position.z);
        }

        if (hasStopped && playerController != null)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, 0);
        }
    }

    // OnCollisionEnter now has NO parenting. We will rely on the HighFriction material.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasStopped || !collision.gameObject.CompareTag("Player")) return;

        playerController = collision.gameObject.GetComponent<PlayerControl>();
        if (playerController != null)
        {
            playerController.SetMovement(false); // Just take control, don't parent.
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (playerController != null && collision.gameObject.CompareTag("Player"))
        {
            // Just give control back. No parenting to worry about.
            rb.linearVelocity = Vector2.zero; // Stop the cloud
            playerController.SetMovement(true);
            playerController = null;
        }
    }
}