using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Movement Stuff
    [Header("Movement")]
   [SerializeField]private float moveSpeed = 10f;
   [SerializeField] private float acceleration = 62;
   [SerializeField] private float deceleration = 62;

   //Jumping Stuff plus some extra mechanics like coyote time
   [Header("Jump")]
   [SerializeField] private float jumpForce = 13f;
   [SerializeField] private float jumpCutMultiplier = 0.1f; // Cuts height when jump key is released early
   [SerializeField] private float coyoteTime = 0.1f; // little air time after ledge
   [SerializeField] private float jumpBufferTime = 0.05f; //jumping right after landing

    //Ground check stuff
    [Header("Ground Check")]
   [SerializeField] private Transform groundCheckPoint;
   [SerializeField] private Vector2 groundCheckRegion = new Vector2(0.5f,0.1f);
   [SerializeField] private LayerMask groundLayer;

    //Component references and varibles
    [Header("Component References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private float sidewaysInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;


    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        //graceful null exception handling
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update() {
        // coyote time management
        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
        } else {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // jump buffer timer
        jumpBufferCounter -= Time.deltaTime;

        // Flip sprite
        if (sidewaysInput > 0f) {
            spriteRenderer.flipX = false;
        } else if (sidewaysInput < 0f) {
            spriteRenderer.flipX = true;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f) {
            ExecuteJump();
        }

        if (animator != null) {
            animator.SetFloat("Speed", Mathf.Abs(sidewaysInput));
            animator.SetBool("IsGrounded", IsGrounded());
            animator.SetFloat("YVelocity", rb.linearVelocity.y);
        }
    }

    private void FixedUpdate() {
        float targetSpeed = sidewaysInput * moveSpeed;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);
    }

    private void OnMove(InputValue value) {
        Vector2 inputVector = value.Get<Vector2>();
        sidewaysInput = inputVector.x;
    }

    private void OnJump(InputValue value) {
        if (value.isPressed) {
            jumpBufferCounter = jumpBufferTime;
        }

        if (!value.isPressed && rb.linearVelocity.y > 0f) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            coyoteTimeCounter = 0f;
        }
    }

    private void ExecuteJump() {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    private bool IsGrounded() {
        if (groundCheckPoint == null) return false;
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckRegion, 0f, groundLayer);
    }

    private void OnDrawGizmosSelected() {
        if (groundCheckPoint != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckRegion);
        }
    }

}