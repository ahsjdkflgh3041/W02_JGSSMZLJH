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

    protected Color m_originColor;
    protected Color m_attackColor;
    protected Color m_hitColor;
    protected Color m_dieColor;

    protected Transform m_target;
    protected Vector2 m_moveDir = Vector2.one;

    [Header("Status")]
    [SerializeField] protected int m_hp;
    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected float m_detectRange;
    [SerializeField] protected float m_attackRange;

    [Header("Cool Time")]
    [SerializeField] protected float m_checkCoolTime = 0.5f;
    [SerializeField] protected float m_hitCoolTime = 0.5f;
    [SerializeField] protected float m_attackCoolTime = 1f;

    protected bool m_isAttacking;
    protected bool m_isHit;

    #endregion

    #region Public Method

    public void TakeDamage(int damage)
    {
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
    }

    protected void Start()
    {
        m_originColor = m_renderer.color;
        m_attackColor = new Color(255 / 255f, 122 / 255f, 0 / 255f, 255 / 255f);
        m_hitColor = Color.red;
        m_dieColor = new Color(m_originColor.r, m_originColor.g, m_originColor.b, 60 / 255f);

        m_state = State.Idle;
        InvokeRepeating(nameof(CheckTarget), 0f, m_checkCoolTime);
        StartCoroutine(nameof(StateMachine));
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
        Vector3 randVec = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);

        for (float i = 0; i < m_checkCoolTime; i += Time.deltaTime)
        {
            transform.position += randVec * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (randVec.normalized.x < 0 ? -1 : 1);
            transform.localScale = m_moveDir;
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

        Vector3 chaseDir = m_target.transform.position - transform.position;

        if (chaseDir.magnitude > 0.01f)
        {
            transform.position += chaseDir.normalized * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (chaseDir.normalized.x < 0 ? -1 : 1);
            transform.localScale = m_moveDir;

            yield return new WaitForEndOfFrame();
        }

        if (chaseDir.magnitude < m_attackRange)
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

    protected void CheckTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, m_detectRange);
        foreach (Collider2D col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                m_target = col.transform;
                ChangeState(State.Chase);
                return;
            }
        }
        m_target = null;
        ChangeState(State.Idle);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && m_isHit == false)
        {
            // 나중에 player가 호출
            TakeDamage(1);
        }
    }

    #endregion
}
