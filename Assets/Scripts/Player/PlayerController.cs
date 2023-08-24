using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        m_directionX = Input.GetAxisRaw("Horizontal");
        if (!m_desiredJump)
        {
            m_desiredJump = Input.GetButtonDown("Jump");
        }

        m_rigidBody.gravityScale = (m_gravityMultiplier * (-2 * m_jumpHeight) / (m_jumpTimeToApex * m_jumpTimeToApex)) / Physics2D.gravity.y;

        m_onGround = m_ground.GetOnGround();
        m_desiredVelocityX = m_directionX * m_speedX;

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
