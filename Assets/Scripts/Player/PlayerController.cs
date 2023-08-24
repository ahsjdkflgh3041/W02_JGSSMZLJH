using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_rigidBody;
    PlayerGround m_ground;

    [SerializeField] private bool m_enableDoubleJump = true;
    [SerializeField] private float m_speedX;
    [SerializeField] private float m_jumpHeight;
    [SerializeField] private float m_jumpTimeToApex;
    [SerializeField] private float m_gravityMultiplier = 1;

    private Vector2 m_velocity;
    private float m_directionX;
    private bool m_desiredJump;
    private float m_desiredVelocityX;

    private bool m_isDashing;
    private bool m_isJumping;
    private bool m_canJumpAgain = false;
    private bool m_onGround;

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_ground = GetComponent<PlayerGround>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var inputVector = context.ReadValue<Vector2>();
        m_directionX = inputVector.x;
    }

    public void OnJump(InputAction.CallbackContext context)
    { 
        if (context.started)
        {
            m_desiredJump = true;
        }
    }

    private void Update()
    {
        Debug.Log($"x input = {m_directionX}");

        m_rigidBody.gravityScale = (m_gravityMultiplier * (-2 * m_jumpHeight) / (m_jumpTimeToApex * m_jumpTimeToApex)) / Physics2D.gravity.y;

        m_onGround = m_ground.GetOnGround();
        m_desiredVelocityX = (m_directionX == 0 ? 0 : Mathf.Sign(m_directionX)) * m_speedX;
    }

    private void FixedUpdate()
    {
        m_velocity = m_rigidBody.velocity;
        m_velocity.x = m_desiredVelocityX;

        if (m_desiredJump)
        {
            DoAJump();
        }

        m_rigidBody.velocity = m_velocity;

        if (m_onGround)
        {
            m_isJumping = false;
        }
    }

    private void DoAJump()
    {
        if (m_onGround || m_canJumpAgain)
        { 
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * m_rigidBody.gravityScale * m_jumpHeight);

            m_canJumpAgain = m_canJumpAgain == false;

            if (m_velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - m_velocity.y, 0f);
            }
            else if (m_velocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(m_rigidBody.velocity.y);
            }

            m_velocity.y += jumpSpeed;
            m_isJumping = true;
        }

        m_desiredJump = false;
    }
}
