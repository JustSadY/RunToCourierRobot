using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Movement2D))]
    [RequireComponent(typeof(Jump2D))]
    [RequireComponent(typeof(WallJump2D))]
    [RequireComponent(typeof(Dash2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Gravity2D))]
    [RequireComponent(typeof(Inventory))]
    public class PlayerController : MonoBehaviour
    {
        private static readonly int MoveSpeedHash = Animator.StringToHash("moveSpeed");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("verticalSpeed");
        private static readonly int IsGroundedHash = Animator.StringToHash("isGrounded");
        private static readonly int Dash = Animator.StringToHash("Dash");

        [Header("Movement Profiles")] [SerializeField]
        private MovementProfile walkProfile;

        [Header("Air Profiles")] [SerializeField]
        private MovementProfile airProfile;

        [Header("Gravity Profiles")] [SerializeField]
        private GravityProfile defaultGravityProfile;

        [Header("Wall Gravity Profiles")] [SerializeField]
        private GravityProfile wallGravityProfile;

        // Components
        private Movement2D _movement;
        private Jump2D _jump;
        private Dash2D _dash;
        private CapsuleCollider2D _collider;
        private Animator _animator;
        private Rigidbody2D _rigidbody;
        private Gravity2D _gravity;
        private WallJump2D _wallJump;
        private Inventory _inventory;

        // Wall contact info
        private bool _isTouchingWall;
        private sbyte _wallDirection;

        private void Awake()
        {
            _movement = GetComponent<Movement2D>();
            _jump = GetComponent<Jump2D>();
            _dash = GetComponent<Dash2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _gravity = GetComponent<Gravity2D>();
            _wallJump = GetComponent<WallJump2D>();
            _inventory = GetComponent<Inventory>();
        }

        private void FixedUpdate()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            var wallHit = CheckWall();

            _isTouchingWall = (wallHit.right && horizontalInput > 0) || (wallHit.left && horizontalInput < 0);
            _wallDirection = wallHit.right ? (sbyte)1 : wallHit.left ? (sbyte)-1 : (sbyte)0;

            if (IsGrounded() || !_isTouchingWall)
                _gravity.SetGravityProfile(defaultGravityProfile);
            else
                _gravity.SetGravityProfile(wallGravityProfile);

            if (horizontalInput > 0 && wallHit.right)
                horizontalInput = 0;
            if (horizontalInput < 0 && wallHit.left)
                horizontalInput = 0;

            _movement.SetMovementProfile(IsGrounded() ? walkProfile : airProfile);
            _movement.Move(horizontalInput);
        }

        private void Update()
        {
            if (IsGrounded()) _jump.ResetJumpCount();
            HandleJump();
            HandleDash();
            HandleTakeBox();

            _animator.SetFloat(MoveSpeedHash, Mathf.Abs(_rigidbody.linearVelocity.x));
            _animator.SetFloat(VerticalSpeedHash, _rigidbody.linearVelocity.y);
            _animator.SetBool(IsGroundedHash, IsGrounded());
        }

        private void HandleJump()
        {
            if (!Input.GetButtonDown("Jump")) return;
            if (_isTouchingWall && !IsGrounded() && _wallDirection != 0)
            {
                _wallJump.WallJump(_wallDirection);
            }
            else
            {
                _jump.Jump(IsGrounded());
            }
        }


        private void HandleTakeBox()
        {
            if (!Input.GetKeyDown(KeyCode.F))
                return;


            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Item"),
            };

            Collider2D[] results = new Collider2D[5];
            int hitCount = _collider.Overlap(filter, results);

            if (hitCount == 0)
                return;

            _inventory.AddItemToHead(results[0].gameObject);
        }


        private void HandleDash()
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
            Vector2 dashDir = new(Input.GetAxisRaw("Horizontal"), 0);
            if (!(dashDir.magnitude > 0.1f) || !_dash.CanDash(dashDir)) return;
            _animator.SetTrigger(Dash);
            _dash.Dash(dashDir, IsGrounded());
        }

        private bool IsGrounded()
        {
            Vector2 origin = new(_collider.bounds.center.x, _collider.bounds.min.y);
            const float rayDistance = 0.3f;
            int layerMask = LayerMask.GetMask("Ground");
            return Physics2D.Raycast(origin, Vector2.down, rayDistance, layerMask);
        }

        private (bool left, bool right) CheckWall()
        {
            Vector2 origin = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
            const float rayDistance = 0.4f;
            int layerMask = LayerMask.GetMask("Ground");

            bool hitRight = Physics2D.Raycast(origin, Vector2.right, rayDistance, layerMask);
            bool hitLeft = Physics2D.Raycast(origin, Vector2.left, rayDistance, layerMask);

            return (hitLeft, hitRight);
        }

        private void OnDrawGizmos()
        {
            if (!_collider) return;

            Vector2 bottom = new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
            const float rayDistance = 0.4f;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(bottom, bottom + Vector2.right * rayDistance);
            Gizmos.DrawLine(bottom, bottom + Vector2.left * rayDistance);
            Gizmos.DrawLine(bottom, bottom + Vector2.down * rayDistance);
        }
    }
}