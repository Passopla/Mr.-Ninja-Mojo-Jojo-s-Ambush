using System;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int lives = 5;
    public event Action OnPlayerDeath;
    public event Action<int> OnHealthChanged;

    private bool canMove = true;
    private float horizontalInput;
    private bool isTouchingCloud = false;

    public float tapTime = 0.2f; // touches shorter than this are jumps, longer are movement
    private float touchMove;
    private bool jumpedByTouch;
    private readonly float[] touchStart = new float[10];

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    [SerializeField] bool isGrounded;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask isGroundLayer;
    [SerializeField] float groundCheckRadius = 0.2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        OnHealthChanged?.Invoke(lives);
    }

    // --- THIS IS THE FINAL, CORRECTED UPDATE METHOD ---
    void Update()
    {
        // STEP 1: Always check for grounded status, no matter what.
        bool isGroundedByCircle = Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, isGroundLayer);
        isGrounded = isGroundedByCircle || isTouchingCloud;
        anim.SetBool("IsGrounded", isGrounded);

        // TOUCH: hold left/right half of the screen to move, quick tap anywhere to jump.
        touchMove = 0;
        bool touchJump = false;
        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Ended && Time.time - touchStart[t.fingerId % 10] < tapTime) touchJump = true;
            else if (t.phase == TouchPhase.Began) touchStart[t.fingerId % 10] = Time.time;
            if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled && Time.time - touchStart[t.fingerId % 10] >= tapTime)
                touchMove = t.position.x < Screen.width * 0.5f ? -1 : 1;
        }

        // STEP 2: Handle horizontal input based on whether we can move.
        if (canMove)
        {
            // If we can move, read the player's input.
            horizontalInput = Input.GetAxis("Horizontal");
            if (touchMove != 0) horizontalInput = touchMove;
            if (horizontalInput != 0) sr.flipX = (horizontalInput < 0);
            anim.SetFloat("MoveSpeed", Mathf.Abs(horizontalInput));
        }
        else
        {
            // If we can't move (because we are on the cloud), force input to zero.
            horizontalInput = 0;
        }

        // STEP 3: Always check for jump input, regardless of canMove status.
        // The check will only pass if isGrounded is true.
        if ((Input.GetButtonDown("Jump") || touchJump) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpedByTouch = touchJump;
        }

        // JUMP PHYSICS TWEAKS
        if (rb.linearVelocity.y <= 0) jumpedByTouch = false;
        if (rb.linearVelocity.y < 0) { rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime); }
        else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump") && !jumpedByTouch) { rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime); }
    }

    private void FixedUpdate()
    {
        // This is now always correct. If on the cloud, horizontalInput will be 0.
        if (canMove)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null) { Gizmos.color = isGrounded ? Color.green : Color.red; Gizmos.DrawWireSphere(GroundCheck.position, groundCheckRadius); }
    }

    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("CloudPlatform")) { isTouchingCloud = true; } }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag("CloudPlatform")) { isTouchingCloud = false; } }

    public void SetMovement(bool isEnabled) { canMove = isEnabled; }
    public void TakeDamage(int damageAmount) { lives -= damageAmount; OnHealthChanged?.Invoke(lives); if (lives <= 0) { lives = 0; OnPlayerDeath?.Invoke(); Destroy(gameObject); } }
    public void AddLife(int amount) { lives += amount; OnHealthChanged?.Invoke(lives); }
}