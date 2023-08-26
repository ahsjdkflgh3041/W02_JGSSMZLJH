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

    Rigidbody2D m_rigidBody;
    BoxCollider2D m_boxCollider;
    PlayerGround m_ground;
    PlayerAttack m_attack;
    PlayerInput m_input;

    [Header("Move")]
    [SerializeField] private float m_speedX;
    [Header("Jump")]
    [SerializeField] private bool m_enableDoubleJump = true;
    [SerializeField] private bool m_enableJumpCutoff = true;
    [SerializeField] private float m_jumpHeight;
    [SerializeField] private float m_jumpTimeToApex;
    [SerializeField] private float m_jumpCutoffGravity;
    [SerializeField] private float m_downwardGravity = 1;
    [SerializeField] private float m_defaultGravity = 1;
    [SerializeField] private float m_maxFallSpeed = 100f;
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
    [SerializeField] private float m_smashBulletTimeSlower;
    [Header("Input")]
    [SerializeField] private float m_jumpBuffer = 0.2f;
    [SerializeField] private float m_smashCriterionTime = 0.2f;

    private Vector2 m_velocity;
    private float m_directionX;
    private float m_desiredVelocityX;
    private bool m_desiredJump;
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
    private bool m_smashInput;
    private float m_smashInputTime;
    private float m_smashStartTime;
    private Vector2 m_smashStartPosition;
    private Vector2 m_smashTargetPosition;
    private float m_defalutTimeScale;
    private float m_lastVelocityY;

    private bool m_isJumping;
    private bool m_canJumpAgain = false;
    private bool m_canJumpOrDash = true;
    private bool m_isDashing;
    private bool m_hasPerformedDash;
    private bool m_isSmashing;
    private bool m_hasPerformedSmash;
    private bool m_onBulletTime;
    private bool m_onGround;
    private bool m_hasJumpedThisFrame;

    public bool IsKeyboardAndMouse { get { return m_input.currentControlScheme.Equals(k_keyboardAndMouseString); } }
    public bool OnBulletTime { get { return m_onBulletTime; } }
    public Vector2 DashDirection { get { return m_dashDirection; } }
    public float DashDistance { get { return m_onBulletTime ? m_smashDistance : m_dashDistance; } }

    void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_ground = GetComponent<PlayerGround>();
        m_attack = GetComponent<PlayerAttack>();
        m_input = GetComponent<PlayerInput>();

        m_gravityMultiplier = m_defaultGravity;
        m_defalutTimeScale = Time.timeScale;
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
            //Debug.Log("OnSmash started");
            m_smashInput = true;
            if (!m_onBulletTime && m_attack.CanSmash)
            {
                m_onBulletTime = true;
                m_canJumpOrDash = false;
                m_rigidBody.velocity = new Vector2(m_rigidBody.velocity.x, Mathf.Max(m_rigidBody.velocity.y, m_rigidBody.velocity.y / 4));
            }
            else
            {
                Debug.Log("Cannot smash... : on cooldown");
            }
        }

        if (context.canceled)
        {
            //Debug.Log("OnSmash canceled");
            if (m_onBulletTime)
            {
                m_desiredSmash = true;
                m_onBulletTime = false;
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
        }
    }

    private void Update()
    {
        Time.timeScale = m_onBulletTime ? m_smashBulletTimeSlower : m_defalutTimeScale;

        if (IsKeyboardAndMouse)
        {
            var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_dashDirection = (mousePosition - (Vector2)transform.position).normalized;
        }

        m_rigidBody.gravityScale = (m_gravityMultiplier * -2 * m_jumpHeight) / (m_jumpTimeToApex * m_jumpTimeToApex * Physics2D.gravity.y);

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
        if (m_onGround)
        {
            m_canJumpAgain = false;
        }
        m_desiredVelocityX = (m_directionX == 0 ? 0 : Mathf.Sign(m_directionX)) * m_speedX;
    }

    private void FixedUpdate()
    {
        m_hasJumpedThisFrame = false;

        m_velocity = m_rigidBody.velocity;

        if (m_onBulletTime)
        {
        }
        else
        {
            m_velocity.x = m_desiredVelocityX;
            m_lastVelocityY = m_velocity.y;
        }

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

        m_rigidBody.velocity = new Vector3(m_velocity.x, Mathf.Clamp(m_velocity.y, -m_maxFallSpeed, float.MaxValue));

        SetGravity();
    }

    private void PerformDash()
    {
        float dashTime = Time.time - m_dashStartTime;

        if (dashTime <= m_dashPreDelay)
        {
            m_velocity = Vector2.zero;
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

                var distance = CalculateDistance(m_dashDistance);
                //Debug.Log($"final distance = {distance}");
                Debug.DrawLine(transform.position, transform.position + (Vector3)m_dashDirection * distance, Color.red, 3);
                transform.Translate(m_dashDirection.normalized * distance, Space.Self);

                m_attack.Dash(m_dashStartPosition, transform.position);
            }

            if (!m_isJumping)
            {
                m_velocity = Vector2.zero;
            }
        }
        else
        {
            m_isDashing = false;
        }
    }
    private void PerformSmash()
    {
        float smashTime = Time.time - m_smashStartTime;

        if (smashTime <= m_smashPreDelay)
        {
            m_velocity = Vector2.zero;
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

                var distance = CalculateDistance(m_smashDistance);
                //Debug.Log($"final distance = {distance}");
                Debug.DrawLine(transform.position, transform.position + (Vector3)m_dashDirection * distance, Color.red, 3);
                transform.Translate(m_dashDirection.normalized * distance, Space.Self);

                m_attack.Smash(m_smashStartPosition, transform.position);
            }

            if (!m_isJumping)
            {
                m_velocity = Vector2.zero;
            }
        }
        else
        {
            var displacement = (Vector2)transform.position - m_dashStartPosition;
            //Debug.Log($"Dash Displacement : {displacement} = {displacement.magnitude}");
            m_isSmashing = false;
        }
    }
    private void DoAJump()
    {
        if (m_onGround || m_canJumpAgain)
        {
            m_desiredJump = false;
            m_jumpBufferCounter = 0;
            m_canJumpAgain = m_enableDoubleJump && m_canJumpAgain == false;

            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * m_rigidBody.gravityScale * m_jumpHeight / m_gravityMultiplier);

            if (m_velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - m_velocity.y, 0f);
            }
            else if (m_velocity.y < 0f)
            {
                jumpSpeed += Mathf.Abs(m_rigidBody.velocity.y);
            }

            m_velocity.y += jumpSpeed;
            //Debug.Log($"jump speed = {jumpSpeed}, velocity y = {m_velocity.y}");
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
            m_dashStartTime = Time.time;
        }
        else
        {
            Debug.Log($"Cannot Dash... Stamina = {m_attack.CurrentStamina}");
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
        m_smashStartTime = Time.time;
    }

    private void SetGravity()
    { 
        if (m_hasJumpedThisFrame) { return; }

        if (m_isDashing)
        {
            m_gravityMultiplier = m_dashGravity * k_defalutFixedTimestep / Time.fixedDeltaTime;
            return;
        }
        else if (m_isSmashing)
        {
            m_gravityMultiplier = m_smashGravity * k_defalutFixedTimestep / Time.fixedDeltaTime;
            return;
        }

        if (m_onBulletTime)
        {
            m_gravityMultiplier = m_defaultGravity / 2;
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
            if (m_onGround)
            {
                m_isJumping = false;
            }

            m_gravityMultiplier = m_defaultGravity;
        }
        //Debug.Log($"isJumping = {m_isJumping}, jumpInput = {m_jumpInput} : Multiplier = {m_gravityMultiplier}");
    }

    float CalculateDistance(float distance)
    {
        var horizontal = 0.95f * m_boxCollider.size.x * transform.lossyScale.x / 2;
        var vertical = 0.95f * m_boxCollider.size.y * transform.lossyScale.y / 2;
        var position = (Vector2)transform.position;
        var dashVector = m_dashDirection * distance;

        var upRightPosition = position + Vector2.right * horizontal + Vector2.up * vertical;
        var upLeftPosition = position + Vector2.left * horizontal + Vector2.up * vertical;
        var downRightPosition = position + Vector2.right * horizontal + Vector2.down * vertical;
        var downLeftPosition = position + Vector2.left * horizontal + Vector2.down * vertical;


        var layer = m_ground.GetLayer();

        var distances = new List<float>();
        distances.Add(Mathf.Min(distance, Vector2.Distance(upRightPosition, Physics2D.Linecast(upRightPosition, upRightPosition + dashVector, layer).point)));
        distances.Add(Mathf.Min(distance, Vector2.Distance(upLeftPosition, Physics2D.Linecast(upLeftPosition, upLeftPosition + dashVector, layer).point)));
        distances.Add(Mathf.Min(distance, Vector2.Distance(downRightPosition, Physics2D.Linecast(downRightPosition, downRightPosition + dashVector, layer).point)));
        distances.Add(Mathf.Min(distance, Vector2.Distance(downLeftPosition, Physics2D.Linecast(downLeftPosition, downLeftPosition + dashVector, layer).point)));

        //distances.ForEach(d => Debug.Log($"distance = {d}"));
        return distances.Min();

        //var start = (Vector2)transform.position;
        //var end = start + m_dashDirection * m_dashDistance;
        //var layer = m_ground.GetLayer();
        //Debug.DrawLine(start, end, Color.red, 3);
        //return Physics2D.Linecast(start, end, layer).distance;
    }
}
