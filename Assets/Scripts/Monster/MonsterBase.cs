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
        Idle, Chase, Attack, Hit, Die,
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

    [Header("Cool Time")]
    protected float m_checkCoolTime = 0.5f;
    protected float m_hitCoolTime = 0.01f;
    [SerializeField] protected float m_attackCoolTime = 1f;

    protected bool m_isAttacking;
    protected bool m_isHit;
    protected bool m_isDead;

    #endregion

    #region Public Method

    public void TakeDamage(int damage)
    {
        if (Health <= 0)
            return;

        Health -= damage;
        SetState(State.Hit);
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

    protected void Start()
    {
        m_originColor = m_renderer.color;
        m_attackColor = new Color(255 / 255f, 122 / 255f, 0 / 255f, 255 / 255f);
        m_hitColor = Color.red;
        m_dieColor = new Color(m_originColor.r, m_originColor.g, m_originColor.b, 60 / 255f);

        m_state = State.Idle;
        InvokeRepeating(nameof(DetectTarget), 0f, m_checkCoolTime);
        StartCoroutine(nameof(StateMachine));
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

    protected void SetState(State _state)
    {
        StopAllCoroutines();

        m_isAttacking = false;
        m_isHit = false;
        Utility.ChangeColor(m_renderer, m_originColor);

        ChangeState(_state);
        StartCoroutine(nameof(StateMachine));
    }

    #endregion

    #region Monster State Define

    protected IEnumerator Idle()
    {
        m_moveDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);

        for (float i = 0; i < m_checkCoolTime; i += Time.deltaTime)
        {
            transform.position += m_moveDir * m_moveSpeed * Time.deltaTime;
            this.m_moveDir.x = (m_moveDir.normalized.x < 0 ? -1 : 1);
            transform.localScale = new Vector3(this.m_moveDir.x, 1, 1);
            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator Chase()
    {
        if (m_isHit == true)
        {
            ChangeState(State.Hit);
            yield break;
        }

        if (m_target == null)
        {
            ChangeState(State.Idle);
            yield break;
        }

        m_moveDir = m_target.transform.position - transform.position;

        if (m_moveDir.magnitude > 0.01f)
        {
            transform.position += m_moveDir.normalized * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (m_moveDir.normalized.x < 0 ? -1 : 1);
            transform.localScale = new Vector3(this.m_moveDir.x, 1, 1);

            yield return new WaitForEndOfFrame();
        }

        if (m_moveDir.magnitude < m_attackRange)
            ChangeState(State.Attack);
    }

    protected virtual IEnumerator Attack()
    {
        if (m_isAttacking == true)
        {
            ChangeState(State.Chase);
            yield break;
        }

        if (m_isHit == true)
        {
            ChangeState(State.Hit);
            yield break;
        }

        m_isAttacking = true;
        m_originColor = m_renderer.color;
        Utility.ChangeColor(m_renderer, m_attackColor);

        yield return new WaitForSeconds(m_attackCoolTime);
        Utility.ChangeColor(m_renderer, m_originColor);
        m_isAttacking = false;
        ChangeState(State.Chase);
    }

    protected IEnumerator Hit()
    {
        m_isHit = true;

        m_originColor = m_renderer.color;
        Utility.ChangeColor(m_renderer, m_hitColor);

        yield return new WaitForSeconds(m_hitCoolTime);
        Utility.ChangeColor(m_renderer, m_originColor);

        m_isHit = false;

        if (Health > 0)
            ChangeState(State.Chase);
        else
            SetState(State.Die);
    }

    protected IEnumerator Die()
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
                ChangeState(State.Chase);
                return;
            }
        }

        m_target = null;
        ChangeState(State.Idle);
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
