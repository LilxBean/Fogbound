using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //tutorial https://www.youtube.com/watch?v=O6VX6Ro7EtA

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private float _horizontal;
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _jumpingPower = 16f;
    private bool _isFacingRight;
    private bool _isWallSliding;
    private float _wallSlidingSpeed = 2f;
    private bool _isWallJumping;
    private float _wallJumpingDirection;
    private float _wallJumpingTime = 0.2f;
    private float _wallJumpingCounter;
    private float _wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 _wallJumpingPower = new Vector2(8f, 16f);
    private bool _isWallClimbing;

    private void Update()
    {
        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
        if(!_isFacingRight && _horizontal > 0f && !_isWallJumping)
        {
            Flip();
        }
        else if(_isFacingRight && _horizontal < 0f && !_isWallJumping)
        {
            Flip();
        }

        WallSlide();
        WallJump();
        ClimbWall();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && IsGrounded())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpingPower);
        }

        if(context.canceled && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f); ;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(_wallCheck.position, 0.2f, _wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && _horizontal != 0f && !_isWallJumping)
        {
            _isWallSliding = true;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -_wallSlidingSpeed, float.MaxValue));
        }

        else 
        { 
            _isWallSliding = false; 
        }
    }

    private void WallJump()
    {
        if(_isWallSliding)
        {
            _isWallJumping = false;
            _wallJumpingDirection = transform.localScale.x;
            _wallJumpingCounter = _wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            _wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            _rb.velocity = new Vector2(_wallJumpingDirection * _wallJumpingPower.x, _wallJumpingPower.y);
            _wallJumpingCounter = 0f;

            if(transform.localScale.x != _wallJumpingDirection)
            {
                _isFacingRight = !_isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), _wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        _isWallJumping = false;
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void ClimbWall()
    {
        //if you run into the wall start climbing to the left
        if (IsWalled() && IsGrounded() && _horizontal < 0f)
        {
            //put delay
            _isWallClimbing = true;
            _rb.velocity = new Vector2(_rb.velocity.x, _speed);
        }
        //wallclimb to the right
        else if (IsWalled() && IsGrounded() && _horizontal > 0f)
        {
            //put delay
            _isWallClimbing = true;
            _rb.velocity = new Vector2(_rb.velocity.x, _speed);
        }
        else
        {
            _isWallClimbing = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
    }
}
