using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterBase : MonoBehaviour, IDamagable
{
    #region Public Variables

    public int Health { get { return m_hp; } set { m_hp = value; } }

    #endregion

    #region Protected Variables

    protected enum State
    {
        IdleState, ChaseState, AttackState, HitState, DieState,
    }
    [SerializeField] protected State m_state;

    protected SpriteRenderer m_renderer;
    protected SpriteRenderer[] m_renderers;
    protected Collider2D m_collider;
    protected MonsterGround m_ground;

    protected Color m_originColor;
    protected Color m_attackColor;
    protected Color m_hitColor;
    protected Color m_dieColor;

    protected Transform m_target;
    protected Vector3 m_moveDir = Vector3.one;

    [Header("Status")]
    [SerializeField] protected int m_hp;
    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected float m_detectRange;
    [SerializeField] protected float m_attackRange;
    [SerializeField] protected int m_attackPower;

    [Header("Time")]
    protected float m_detectCoolTime = 0.5f;
    protected float m_hitCoolTime = 0.5f;
    [SerializeField] protected float m_attackingTime = 1f;
    [SerializeField] protected float m_attackCoolTime;
    protected float m_attackCoolTimeCounter;

    protected bool m_isAttacking;
    protected bool m_canAttack = true;
    protected bool m_isHitting;
    protected bool m_isDead;

    #endregion

    #region Public Method

    public void TakeDamage(int damage)
    {
        if (Health <= 0)
            return;

        Health -= damage;
        ForceChangeState(State.HitState);
    }

    #endregion

    #region Protected Method

    protected void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_renderers = GetComponentsInChildren<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        m_ground = GetComponent<MonsterGround>();
    }

    protected virtual void Start()
    {
        m_originColor = m_renderer.color;
        m_attackColor = new Color(255 / 255f, 122 / 255f, 0 / 255f, 255 / 255f);
        m_hitColor = Color.red;
        m_dieColor = new Color(m_originColor.r, m_originColor.g, m_originColor.b, 60 / 255f);

        m_state = State.IdleState;
        InvokeRepeating(nameof(DetectTarget), 0f, m_detectCoolTime);
        StartCoroutine(nameof(StateMachine));
    }

    protected virtual void Update()
    {
    }

    protected void LateUpdate()
    {
        if (m_ground.GetOnGround() == true)
            Turn();
    }

    #region State Define

    protected IEnumerator StateMachine()
    {
        do
        {
            yield return StartCoroutine(m_state.ToString());
        } while (Health > 0);
    }

    protected void ChangeState(State _state)
    {
        m_state = _state;
    }

    protected void ForceChangeState(State _state)
    {
        StopAllCoroutines();
        Init();
        ChangeState(_state);
        StartCoroutine(nameof(StateMachine));
    }

    protected virtual void Init()
    {
        m_isAttacking = false;
        m_isHitting = false;
        Utility.ChangeColor(m_renderer, m_originColor);
        StartCoroutine(nameof(WaitAttackCoolTime));
    }

    #endregion

    #region Monster State Define

    protected IEnumerator IdleState()
    {
        m_moveDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);

        for (float i = 0; i < m_detectCoolTime; i += Time.deltaTime)
        {
            transform.position += m_moveDir * m_moveSpeed * Time.deltaTime;
            this.m_moveDir.x = (m_moveDir.normalized.x < 0 ? -1 : 1);
            transform.localScale = new Vector3(this.m_moveDir.x, 1, 1);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator ChaseState()
    {
        if (m_target == null)
        {
            ChangeState(State.IdleState);
            yield break;
        }

        m_moveDir = m_target.transform.position - transform.position;

        if (m_moveDir.magnitude > 0.5f)
        {
            transform.position += m_moveDir.normalized * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (m_moveDir.normalized.x < 0 ? -1 : 1);
            transform.localScale = new Vector3(this.m_moveDir.x, 1, 1);

            yield return new WaitForEndOfFrame();
        }

        if (m_moveDir.magnitude < m_attackRange)
            ChangeState(State.AttackState);
    }

    protected virtual IEnumerator AttackState()
    {
        if (m_canAttack == false || m_isAttacking == true)
        {
            ChangeState(State.ChaseState);
            yield break;
        }

        if (m_isHitting == true)
        {
            ChangeState(State.HitState);
            yield break;
        }

        m_isAttacking = true;
        m_canAttack = false;
        StartCoroutine(nameof(WaitAttackCoolTime));
        Utility.ChangeColor(m_renderer, m_attackColor);

        yield return new WaitForSeconds(m_attackingTime);
        m_isAttacking = false;
        Utility.ChangeColor(m_renderer, m_originColor);

        ChangeState(State.ChaseState);
    }

    protected IEnumerator WaitAttackCoolTime()
    {
        if (m_canAttack == true) yield break;
            
        while (m_attackCoolTimeCounter < m_attackCoolTime)
        {
            m_attackCoolTimeCounter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_attackCoolTimeCounter = 0;
        m_canAttack = true;
    }

    protected IEnumerator HitState()
    {
        m_isHitting = true;
        Utility.ChangeColor(m_renderer, m_hitColor);

        yield return new WaitForSeconds(m_hitCoolTime);
        Utility.ChangeColor(m_renderer, m_originColor);
        m_isHitting = false;

        if (Health > 0)
            ChangeState(State.ChaseState);
        else
            ForceChangeState(State.DieState);
    }

    protected IEnumerator DieState()
    {
        Utility.ChangeColor(m_renderer, m_dieColor);
        m_collider.isTrigger = true;

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    #endregion

    protected void DetectTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, m_detectRange, LayerMask.GetMask("Player"));

        if (cols != null)
        {
            foreach (Collider2D col in cols)
            {
                m_target = col.transform;
                ChangeState(State.ChaseState);
                return;
            }
        }
        else
        {
            m_target = null;
            ChangeState(State.IdleState);
        }
    }

    protected void Turn()
    {
        m_moveDir = new Vector3(m_moveDir.x * -1, m_moveDir.y * -1, m_moveDir.z);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }

    #endregion
}
