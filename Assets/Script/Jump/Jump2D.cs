using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Jump2D : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [Header("Jump Settings")]
    [Tooltip("Force applied when jumping (higher = higher jumps)")]
    [FormerlySerializedAs("JumpForce")]
    [SerializeField]
    private float jumpForce = 5f;

    [Tooltip("Maximum number of jumps allowed (1 = single jump, 2 = double jump, etc.)")]
    [FormerlySerializedAs("MaxJumpCount")]
    [SerializeField]
    private int maxJumpCount = 2;

    [Header("Coyote Time")]
    [Tooltip("Grace period after leaving ground where player can still jump (in seconds)")]
    [FormerlySerializedAs("MaxCoyoteTime")]
    [SerializeField]
    private float maxCoyoteTime = 0.1f;

    private int _jumpCount;
    private float _coyoteTimeRemaining;
    private bool _wasGrounded;
    private bool _isGrounded;
    private bool _isJumping;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _coyoteTimeRemaining = maxCoyoteTime;
    }

    public void Jump(bool isGrounded)
    {
        switch (isGrounded)
        {
            case true when !_wasGrounded:
                OnLanded();
                break;
            case false when _wasGrounded:
                _coyoteTimeRemaining = maxCoyoteTime;
                break;
            case false when !_isJumping:
                _coyoteTimeRemaining -= Time.deltaTime;
                _coyoteTimeRemaining = Mathf.Max(0f, _coyoteTimeRemaining);
                break;
        }

        _wasGrounded = isGrounded;
        _isGrounded = isGrounded;

        if (!CanJump()) return;

        PerformJump();
    }

    private void OnLanded()
    {
        _jumpCount = 0;
        _isJumping = false;
        _coyoteTimeRemaining = maxCoyoteTime;
    }

    private bool CanJump()
    {
        return _jumpCount < maxJumpCount &&
               (_isGrounded || _coyoteTimeRemaining > 0f);
    }


    private void PerformJump()
    {
        _jumpCount++;
        _isJumping = true;
        _coyoteTimeRemaining = 0f;

        Vector2 velocity = _rigidbody.linearVelocity;
        velocity.y = 0f;
        _rigidbody.linearVelocity = velocity;

        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void ResetJumpCount() => _jumpCount = 0;

    public int GetJumpCount() => _jumpCount;

    public void SetJumpCount(int count) => _jumpCount = count;
}