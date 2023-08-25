using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D m_rigidBody;
    PlayerGround m_ground;
    PlayerAttack m_attack;

    [Header("Move")]
    [SerializeField] private float m_speedX;
    [Header("Jump")]
    [SerializeField] private bool m_enableDoubleJump = true;
    [SerializeField] private float m_jumpHeight;
    [SerializeField] private float m_jumpTimeToApex;
    [SerializeField] private float m_defaultGravity = 1;
    [Header("Dash")]
    [SerializeField] private float m_dashDistance;
    [SerializeField] private float m_dashPreDelay;
    //[SerializeField] private float m_dashTime;
    [SerializeField] private float m_dashPostDelay;
    [Header("Input")]
    [SerializeField] private float m_jumpBuffer = 0.2f;
    [SerializeField] private float m_dashCriterionTime = 0.2f;

    private Vector2 m_velocity;
    private float m_directionX;
    private float m_desiredVelocityX;
    private bool m_desiredJump;
    private float m_gravityMultiplier;
    private bool m_desiredDash;
    // Temp serialization
    [Header("temp serialization")] [SerializeField] private Vector2 m_dashDirection;
    private Vector2 m_dashStartPosition;
    private Vector2 m_dashTargetPosition;
    private float m_dashStartTime;
    private float m_jumpBufferCounter;
    private float m_dashInputTime;
    private bool m_dashInput;

    private bool m_isDashing;
    private bool m_hasPerformedDash;
    private bool m_isJumping;
    private bool m_canJumpAgain = false;
    private bool m_onGround;

   

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_ground = GetComponent<PlayerGround>();
        m_attack = GetComponent<PlayerAttack>();

        m_gravityMultiplier = m_defaultGravity;
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Push Dash");
            m_dashInput = true;
            m_dashInputTime = (float)context.startTime;
        }

        if (context.canceled)
        {
            if (Time.realtimeSinceStartup < m_dashInputTime + m_dashCriterionTime)
            {
                m_desiredDash = true;
            }
            else
            {
                Debug.Log($"dash input too long : {Time.realtimeSinceStartup - m_dashInputTime}");
            }

            m_dashInput = false;
        }
    }

    private void Update()
    {
        m_rigidBody.gravityScale = (m_gravityMultiplier * (-2 * m_jumpHeight) / (m_jumpTimeToApex * m_jumpTimeToApex)) / Physics2D.gravity.y;

        if (m_jumpBuffer > 0)
        {
            if (m_desiredJump)
            {
                m_jumpBufferCounter += Time.deltaTime;

                if (m_jumpBufferCounter > m_jumpBuffer)
                {
                    m_desiredJump = false;
                    m_jumpBufferCounter = 0;
                }
            }
        }

        m_onGround = m_ground.GetOnGround();
        m_desiredVelocityX = (m_directionX == 0 ? 0 : Mathf.Sign(m_directionX)) * m_speedX;
    }

    private void FixedUpdate()
    {
        if (m_isDashing)
        {
            PerformDash();
        }
        else
        {
            m_velocity = m_rigidBody.velocity;
            m_velocity.x = m_desiredVelocityX;

            if (m_desiredDash)
            {
                DoADash();
            }
            else if (m_desiredJump)
            {
                DoAJump();
            }
        }

        m_rigidBody.velocity = m_velocity;

        if (m_onGround)
        {
            m_isJumping = false;
        }

        SetGravity();
    }

    private void PerformDash()
    {
        float dashTime = Time.time - m_dashStartTime;
        m_velocity = Vector2.zero;

        if (dashTime <= m_dashPreDelay)
        {

        }
        //else if (dashTime <= m_dashPreDelay + m_dashTime)
        //{
        //    //m_velocity = m_dashDirection.normalized * m_dashDistance / m_dashTime;

        //    m_velocity = Vector2.zero;
        //    transform.position = Vector2.Lerp(m_dashStartPosition, m_dashTargetPosition, dashTime - m_dashPreDelay);
        //}
        else if (dashTime <= m_dashPreDelay + m_dashPostDelay)
        {
            if (!m_hasPerformedDash)
            {
                m_hasPerformedDash = true;
                transform.Translate(m_dashDirection.normalized * m_dashDistance, Space.Self);

                m_attack.Attack(m_dashStartPosition, transform.position);
            }
        }
        else
        {
            var displacement = (Vector2)transform.position - m_dashStartPosition;
            Debug.Log($"Dash Displacement : {displacement} = {displacement.magnitude}");
            m_isDashing = false;
        }
    }
    private void DoAJump()
    {
        if (m_onGround || m_canJumpAgain)
        {
            m_desiredJump = false;
            m_jumpBufferCounter = 0;
            m_canJumpAgain = m_canJumpAgain == false;

            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * m_rigidBody.gravityScale * m_jumpHeight);

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

    }

    private void DoADash()
    {
        Debug.Log("Dash!");
        m_isDashing = true;
        m_desiredDash = false;
        m_hasPerformedDash = false;

        m_dashStartPosition = (Vector2)transform.position;
        m_dashTargetPosition = (Vector2)transform.position + m_dashDirection.normalized * m_dashDistance;
        m_dashStartTime = Time.time;
    }

    private void SetGravity()
    {
        m_gravityMultiplier = m_isDashing ? 0 : m_defaultGravity;
    }
}
