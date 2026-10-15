using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
   private float moveSpeed = 6f;
   private float jumpForce = 8f;
   [SerializeField] private Transform groundCheckPoint;
   [SerializeField] private Vector2 groundCheckRegion = new Vector2(0.5f,0.1f);
   [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private float sidewaysInput;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value) {
        Vector2 inputVector = value.Get<Vector2>();
        sidewaysInput = inputVector.x;
    }

    private void OnJump(InputValue value) {
        if (value.isPressed && IsGrounded()) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate() {
        rb.linearVelocity = new Vector2(sidewaysInput * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded() {
        if (groundCheckPoint == null) return false;

        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckRegion, 0f, groundLayer);
    }
}