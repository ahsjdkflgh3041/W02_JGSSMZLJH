using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const string k_keyboardAndMouseString = "Keyboard&Mouse";
    const float k_almosZero = 0.001f;
    const float k_defalutFixedTimestep = 0.02f;
    const float k_inputCriteriaDegree = 30f;

    Rigidbody2D m_rigidBody;
    BoxCollider2D m_boxCollider;
    PlayerGround m_ground;
    PlayerHealth m_health;
    PlayerAttack m_attack;
    PlayerInput m_input;
    PlayerRayProjector m_rayProjector;
    TimeController m_timeController;

    [Header("Move")]
    [SerializeField] private float m_speedX;
    [SerializeField] private float m_speedY;
    [Header("Jump")]
    [SerializeField] private bool m_enableDoubleJump = true;
    [SerializeField] private bool m_enableJumpCutoff = true;
    [SerializeField] private float m_jumpHeight;
    [SerializeField] private float m_jumpTimeToApex;
    [SerializeField] private float m_jumpCutoffGravity;
    [SerializeField] private float m_downwardGravity = 1;
    [SerializeField] private float m_defaultGravity = 1;
    [SerializeField] private float m_maxFallSpeed = 100f;
    [SerializeField] private float m_wallJumpXModifier = 1;
    [SerializeField] private float m_coyoteTime;
    [Header("Dash")]
    [SerializeField] private float m_dashDistance;
    [SerializeField] private float m_dashPreDelay;
    [SerializeField] private float m_dashGravity;
    //[SerializeField] private float m_dashTime;
    [SerializeField] private float m_dashPostDelay;
    [Header("Smash")]
    [SerializeField] private float m_smashDistance;
    [SerializeField] private float m_smashPreDelay;
    [SerializeField] private float m_smashGravity;
    [SerializeField] private float m_smashPostDelay;
    [Header("Input")]
    [SerializeField] private float m_jumpBuffer = 0.2f;
    [SerializeField] private float m_smashCriterionTime = 0.2f;
    [SerializeField] private int m_ignoreRayResult = 0;

    private Vector2 m_velocity;
    private float m_directionX;
    private float m_directionY;
    private float m_desiredVelocityX;
    private bool m_desiredJump;
    private float m_coyoteTimeCounter;
    private bool m_canCoyoteWall;
    private int m_coyoteWallDirection;
    private float m_gravityCoefficient;
    private float m_gravityMultiplier;
    private bool m_desiredDash;
    private bool m_desiredSmash;
    // Temp serialization
    [Header("temp serialization")] [SerializeField] private Vector2 m_dashDirection;
    private Vector2 m_dashStartPosition;
    private Vector2 m_dashTargetPosition;
    private float m_dashStartTime;
    private bool m_jumpInput;
    private float m_jumpBufferCounter;
    private bool m_hasPerformedWallJump;
    private bool m_smashInput;
    private float m_smashInputTime;
    private float m_smashStartTime;
    private Vector2 m_smashStartPosition;
    private Vector2 m_smashTargetPosition;
    private float m_lastVelocityY;

    private bool m_isJumping;
    private bool m_canJumpAgain = false;
    private bool m_canJumpOrDash = true;
    private bool m_isDashing;
    private bool m_hasPerformedDash;
    private bool m_isSmashing;
    private bool m_hasPerformedSmash;
    private bool m_onGround;
    private bool m_onWall;
    private bool m_wasOnWall;
    private bool m_onRightWall;
    private bool m_onLeftWall;
    private bool m_hasJumpedThisFrame;

    [Header("VCam")]
    [SerializeField] private TestVCamBlend baseVCam;

    public bool IsKeyboardAndMouse { get { return m_input.currentControlScheme.Equals(k_keyboardAndMouseString); } }
    public Vector2 DashDirection { get { return m_dashDirection; } }
    public float IndicatorDistance { get { return m_smashInput && m_attack.CanSmash ? m_smashDistance : m_dashDistance; } }
    private bool OnBulletTime { get { return m_timeController.OnBulletTime; } }
    public bool IsSmashInput { get { return m_smashInput; } }


    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_ground = GetComponent<PlayerGround>();
        m_health = GetComponent<PlayerHealth>();
        m_attack = GetComponent<PlayerAttack>();
        m_input = GetComponent<PlayerInput>();
        m_rayProjector = GetComponent<PlayerRayProjector>();
        m_timeController = FindObjectOfType<TimeController>();

        m_gravityMultiplier = m_defaultGravity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var inputVector = context.ReadValue<Vector2>();
        m_directionX = inputVector.x;

        var yDegree = Mathf.Asin(inputVector.y) * Mathf.Rad2Deg;
        if (yDegree >= k_inputCriteriaDegree) { m_directionY = 1; }
        else if (yDegree <= -k_inputCriteriaDegree) { m_directionY = -1; }
        else { m_directionY = 0; }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_desiredJump = true;
            m_jumpInput = true;
        }
        if (context.canceled)
        {
            m_jumpInput = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            m_desiredDash = true;
        }
    }

    public void OnSmash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            baseVCam.Expand();
            m_smashInput = true;
            if (!OnBulletTime)
            {
                if (m_attack.CanSmash)
                {
                    m_timeController.StartBulletTime();
                    m_canJumpOrDash = false;
                    m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, Mathf.Max(m_rigidBody.velocity.y, m_rigidBody.velocity.y / 4));
                }
            }
            else
            {

            }
        }

        if (context.canceled)
        {
            if (OnBulletTime)
            {
                if (m_attack.CanSmash)
                {
                    m_desiredSmash = true;
                    m_timeController.EndBulletTime();
                    baseVCam.Reduce();
                }
            }
            m_smashInput = false;
        }
    }

    public void OnDashDirection(InputAction.CallbackContext context)
    {
        if (!IsKeyboardAndMouse)
        {
            var inputVector = context.ReadValue<Vector2>();
            if (inputVector != Vector2.zero)
            {
                m_dashDirection = context.ReadValue<Vector2>().normalized;
            }
            CompensateDirection();
        }
    }

    private void Update()
    {
        if (IsKeyboardAndMouse)
        {
            var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_dashDirection = (mousePosition - (Vector2)transform.position).normalized;
            CompensateDirection();
        }

        if (!m_health.IsAlive) { return; }

        m_onGround = m_ground.GetOnGround();
        m_onRightWall = m_ground.GetOnRightWall();
        m_onLeftWall = m_ground.GetOnLeftWall();
        m_onWall = m_ground.GetOnWall();

        m_gravityCoefficient = (-2 * m_jumpHeight) / (m_jumpTimeToApex * m_jumpTimeToApex * Physics2D.gravity.y);
        m_rigidBody.gravityScale = m_gravityMultiplier * m_gravityCoefficient;

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


        if (m_hasPerformedWallJump && (!m_jumpInput || m_onGround || !m_canJumpOrDash))
        {
            m_hasPerformedWallJump = false;   
        }

        if (!m_wasOnWall && m_onWall)
        {
            m_wasOnWall = true;
            m_canCoyoteWall = false;
            m_coyoteWallDirection = m_onRightWall ? -1 : 1;
        }
        if (m_wasOnWall && !m_onWall)
        {
            m_wasOnWall = false;
            if (!m_onGround)
            {
                m_canCoyoteWall = true;
                m_canJumpAgain = true;
            }
        }

        if (m_onGround && m_canCoyoteWall)
        {
            m_canCoyoteWall = false;
        }
        if (m_onGround || m_onWall)
        {
            m_canJumpAgain = false;
            m_coyoteTimeCounter = 0;
        }
        else if (!m_onGround && !m_onWall)
        {
            if (m_coyoteTimeCounter >= 0)
            {
                m_coyoteTimeCounter += Time.deltaTime;
            }
        }
        //Debug.Log($"CoyoteTimeCounter : {m_coyoteTimeCounter}");
        m_desiredVelocityX = (m_directionX == 0 ? 0 : Mathf.Sign(m_directionX)) * m_speedX;
    }

    void CompensateDirection()
    {
        var cirteriaDegree = k_inputCriteriaDegree;
        if (
            ((m_ground.OnGroundRight && m_dashDirection.x > 0) || (m_ground.OnGroundLeft && m_dashDirection.x < 0))
            && m_dashDirection.y < 0 && Vector2.Angle(Vector2.down, m_dashDirection) > cirteriaDegree
            )
        {
            m_dashDirection = (new Vector2(m_dashDirection.x, 0)).normalized;
        }
        else if (
            (((m_ground.OnRightWallUp && m_dashDirection.y > 0) || (m_ground.OnRightWallDown && m_dashDirection.y < 0)) && m_dashDirection.x > 0 && Vector2.Angle(Vector2.right, m_dashDirection) > cirteriaDegree)
            || (((m_ground.OnLeftWallUp && m_dashDirection.y > 0) || (m_ground.OnLeftWallDown && m_dashDirection.y < 0)) && m_dashDirection.x < 0 && Vector2.Angle(Vector2.left, m_dashDirection) > cirteriaDegree)
                )
        {
            m_dashDirection = (new Vector2(0, m_dashDirection.y)).normalized;
        }
    }

    private void FixedUpdate()
    {
        if (!m_health.IsAlive) { return; }

        m_hasJumpedThisFrame = false;

        m_velocity = m_rigidBody.velocity;

        SetVelocity();

        if (m_isDashing)
        {
            PerformDash();
        }
        else if (m_isSmashing)
        {
            PerformSmash();
        }

        if (m_desiredSmash)
        {
            DoASmash();
        }
        if (m_canJumpOrDash)
        { 
            if (m_desiredDash)
            {
                DoADash();
            }
            else if (m_desiredJump)
            {
                DoAJump();
                m_hasJumpedThisFrame = true;
            }
                
            if (m_desiredSmash)
            {
                m_desiredSmash = false;
            }
        }


        m_rigidBody.velocity = new Vector2(m_velocity.x, Mathf.Max(m_velocity.y, -m_maxFallSpeed));

        SetGravity();
    }
    private void SetVelocity()
    {
        if (m_onWall)
        {
            if (m_hasPerformedWallJump)
            {
                return;
            }

            if (m_directionY > 0)
            {
                m_velocity.y = m_speedY;
            }
            else if (m_directionY == 0 && ((m_onRightWall && m_desiredVelocityX > 0) || (m_onLeftWall && m_desiredVelocityX < 0)))
            {
                m_velocity.y = 0;
            }
            else
            {
                m_velocity.y = -m_speedY;
            }
        }

        if (OnBulletTime)
        {

        }
        else
        {
            if (!m_hasPerformedWallJump)
            {
                m_velocity.x = m_desiredVelocityX;
            }
            m_lastVelocityY = m_velocity.y;
        }
    }
    private void PerformDash()
    {
        m_velocity = Vector2.zero;
        float dashTime = Time.realtimeSinceStartup - m_dashStartTime;

        if (dashTime <= m_dashPreDelay)
        {

        }
        //else if (dashTime <= m_dashPreDelay + m_dashTime)
        //{
        //    
        //}
        else if (dashTime <= m_dashPreDelay + m_dashPostDelay)
        {
            if (!m_hasPerformedDash)
            {
                m_hasPerformedDash = true;
                m_canJumpOrDash = true;

                var distance = m_rayProjector.CalculateDistance(m_dashDirection, m_dashDistance, m_ground.GetLayer(), m_ignoreRayResult);
                //Debug.Log($"final distance = {distance}");
                Debug.DrawLine(transform.position, transform.position + (Vector3)m_dashDirection * distance, Color.red, 3);
                transform.Translate(m_dashDirection.normalized * distance, Space.Self);

                m_attack.Dash(m_dashStartPosition, transform.position);

                m_canJumpAgain = true;
            }

            if (!m_isJumping)
            {
                m_velocity.y = -m_dashGravity;
            }
        }
        else
        {
            m_isDashing = false;
        }
    }
    private void PerformSmash()
    {
        m_velocity = Vector2.zero;
        float smashTime = Time.realtimeSinceStartup - m_smashStartTime;

        if (smashTime <= m_smashPreDelay)
        {

        }
        //else if (smashTime <= m_smashPreDelay + m_smashTime)
        //{

        //}
        else if (smashTime <= m_smashPreDelay + m_smashPostDelay)
        {
            if (!m_hasPerformedSmash)
            {
                m_hasPerformedSmash = true;
                m_canJumpOrDash = true;

                var distance = m_rayProjector.CalculateDistance(m_dashDirection, m_smashDistance, m_ground.GetLayer(), m_ignoreRayResult);
                //Debug.Log($"final distance = {distance}");
                Debug.DrawLine(transform.position, transform.position + (Vector3)m_dashDirection * distance, Color.red, 3);
                transform.Translate(m_dashDirection.normalized * distance, Space.Self);

                m_attack.Smash(m_smashStartPosition, transform.position);

                m_canJumpAgain = true;
            }

            if (!m_isJumping)
            {
                m_velocity.y = -m_smashGravity;
            }
        }
        else
        {
            m_isSmashing = false;
        }
    }
    private void DoAJump()
    {
        var canCoyote = (m_coyoteTimeCounter > Time.fixedDeltaTime && m_coyoteTimeCounter < m_coyoteTime);
        if (m_onGround || m_canJumpAgain || m_onWall || canCoyote)
        {
            m_desiredJump = false;
            m_jumpBufferCounter = 0;
            m_coyoteTimeCounter = -1;
            m_canJumpAgain = m_enableDoubleJump && m_canJumpAgain == false;

            float jumpSpeed = m_gravityMultiplier == 0 ?
                Mathf.Sqrt(-2f * Physics2D.gravity.y * m_defaultGravity * m_gravityCoefficient * m_jumpHeight / m_defaultGravity)
                : Mathf.Sqrt(-2f * Physics2D.gravity.y * m_rigidBody.gravityScale * m_jumpHeight / m_gravityMultiplier);

            //Debug.Log($"jumpSpeed(pre) = {jumpSpeed}");

            var canCoyoteWall = (m_canCoyoteWall && canCoyote);
            if (m_onWall || canCoyoteWall)
            {
                var direction = m_onRightWall ? -1 : 1;
                if (canCoyoteWall)
                {
                    m_canCoyoteWall = false;
                    direction = m_coyoteWallDirection;
                    m_coyoteWallDirection = 0;
                    m_canJumpAgain = true;
                }
                m_velocity.x = (m_speedX * direction + m_desiredVelocityX) * m_wallJumpXModifier;
                m_velocity.y = jumpSpeed;
                m_hasPerformedWallJump = true;
            }
            else
            {
                if (m_velocity.y > 0f)
                {
                    jumpSpeed = Mathf.Max(jumpSpeed - m_velocity.y, 0f);
                }
                else if (m_velocity.y < 0f)
                {
                    jumpSpeed += Mathf.Abs(m_rigidBody.velocity.y);
                }

                m_velocity.y += jumpSpeed;
            }

            //Debug.Log($"jump speed = {jumpSpeed}, velocity = {m_velocity}");
            m_isJumping = true;
        }
    }

    private void DoADash()
    {
        m_desiredDash = false;

        if (m_attack.CanDash)
        {
            m_isDashing = true;
            m_isJumping = false;
            m_canJumpOrDash = false;
            m_hasPerformedDash = false;

            m_dashStartPosition = (Vector2)transform.position;
            m_dashTargetPosition = (Vector2)transform.position + m_dashDirection.normalized * m_dashDistance;
            m_dashStartTime = Time.realtimeSinceStartup;
        }
        else
        {
            //Debug.Log($"Cannot Dash... Stamina = {m_attack.CurrentStamina}");
        }
    }
    private void DoASmash()
    {
        //Debug.Log("DoASmash!");
        m_desiredSmash = false;

        m_isSmashing = true;
        m_isJumping = false;
        m_hasPerformedSmash = false;

        m_smashStartPosition = (Vector2)transform.position;
        m_smashTargetPosition = (Vector2)transform.position + m_dashDirection.normalized * m_smashDistance;
        m_smashStartTime = Time.realtimeSinceStartup;
    }

    private void SetGravity()
    {
        if (m_hasJumpedThisFrame) { return; }

        //if (m_isDashing)
        //{
        //    if (!m_isJumping)
        //    {
        //        m_gravityMultiplier = m_dashGravity * k_defalutFixedTimestep / Time.fixedDeltaTime;
        //        return;
        //    }
        //    else
        //    {
        //    }
        //}
        //else if (m_isSmashing && !m_isJumping)
        //{
        //    m_gravityMultiplier = m_smashGravity * k_defalutFixedTimestep / Time.fixedDeltaTime;
        //    return;
        //}
        if (!m_isJumping && (m_isDashing || m_isSmashing))
        {
            m_gravityMultiplier = 0;
        }

        if (OnBulletTime)
        {
            m_gravityMultiplier = m_defaultGravity / 2;
            return;
        }

        if (m_onWall && !m_hasPerformedWallJump)
        {
            m_gravityMultiplier = 0;
            return;
        }

        if (m_rigidBody.velocity.y > k_almosZero)
        {
            if (m_onGround)
            {
                m_gravityMultiplier = m_defaultGravity;
            }
            else
            {
                if (m_enableJumpCutoff)
                {
                    if (m_isJumping && m_jumpInput)
                    {
                        m_gravityMultiplier = m_defaultGravity;
                    }
                    else
                    {
                        m_gravityMultiplier = m_jumpCutoffGravity;
                    }
                }
                else
                {
                    m_gravityMultiplier = m_defaultGravity;
                }
            }
        }
        else if (m_rigidBody.velocity.y < -k_almosZero)
        {
            if (m_onGround)
            {
                m_gravityMultiplier = m_defaultGravity;
            }
            else
            {
                m_gravityMultiplier = m_downwardGravity;
            }
        }
        else
        {
            if (m_onGround || m_onWall)
            {
                m_isJumping = false;
            }

            m_gravityMultiplier = m_defaultGravity;
        }
        //Debug.Log($"isJumping = {m_isJumping}, jumpInput = {m_jumpInput} : Multiplier = {m_gravityMultiplier}");
    }

}
