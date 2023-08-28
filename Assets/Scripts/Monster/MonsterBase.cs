using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class MonsterBase : MonoBehaviour
{
    #region Protected Variables


    [SerializeField] protected State m_state;

    protected SpriteRenderer m_renderer;
    protected SpriteRenderer[] m_renderers;
    protected Collider2D m_collider;
    protected MonsterHealth m_health;
    protected MonsterMove m_move;

    [Header("Color")]
    protected Color m_originColor;
    protected Color m_attackColor;
    protected Color m_hitColor;
    protected Color m_dieColor;

    protected Transform m_target;
    protected Vector3 m_moveDir;
    protected Vector3 m_targetDir;

    [Header("Status")]
    [SerializeField] public float m_detectRange;
    [SerializeField] protected float m_attackRange;
    [SerializeField] protected int m_attackPower;

    [Header("Time")]
    protected float m_detectCoolTime = 0.5f;
    protected float m_hitCoolTime = 0.5f;
    [SerializeField] protected float m_attackingTime = 0.5f;
    [SerializeField] protected float m_attackCoolTime;
    protected float m_attackCoolTimeCounter;
    protected float m_dieIntervalTime = 0.5f;

    [Header("Drop Item")]
    [SerializeField] GameObject m_dropItem;

    protected bool m_isAttacking;
    protected bool m_canAttack = true;
    protected bool m_isHitting;
    protected bool m_isDead;

    #endregion

    #region Public Method

    public void TakeDamage(int damage)
    {
        if (m_health.Health <= 0)
            return;

        m_health.Health -= damage;
        ForceChangeState(State.HitState);
    }

    #endregion

    #region Protected Method

    protected void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_renderers = GetComponentsInChildren<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        m_health = GetComponent<MonsterHealth>();
        m_move = GetComponent<MonsterMove>();
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

    #region State Define

    protected IEnumerator StateMachine()
    {
        do
        {
            yield return StartCoroutine(m_state.ToString());
        } while (m_health.Health > 0);
    }

    protected void ChangeState(State _state)
    {
        m_state = _state;
    }

    public void ForceChangeState(State _state)
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
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
        m_move.MoveDir = randomDir;

        for (float i = 0; i < m_detectCoolTime; i += Time.deltaTime)
        {
            //m_move.Move();
            yield return null;
        }
    }

    protected IEnumerator ChaseState()
    {
        if (m_target == null)
        {
            ChangeState(State.IdleState);
            yield break;
        }
        m_targetDir = m_target.transform.position - transform.position;

        if (m_targetDir.magnitude > 0.5f)
        {
            m_move.MoveDir = m_targetDir;

            m_move.Move();
            yield return null;
        }

        if (m_targetDir.magnitude < m_attackRange)
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
            yield return null;
        }

        m_attackCoolTimeCounter = 0;
        m_canAttack = true;
    }

    protected IEnumerator HitState()
    {
        m_isHitting = true;
        Utility.ChangeColor(m_renderer, m_hitColor);

        if (m_health.Health <= 0)
        {
            ForceChangeState(State.DieState);
            m_isHitting = false;
            yield break;
        }

        yield return new WaitForSeconds(m_hitCoolTime);
        Utility.ChangeColor(m_renderer, m_originColor);
        m_isHitting = false;

        if (m_health.Health > 0)
            ChangeState(State.ChaseState);
        else
            ForceChangeState(State.DieState);
    }

    protected IEnumerator DieState()
    {
        Utility.ChangeColor(m_renderer, m_dieColor);
        m_collider.isTrigger = true;

        DropItem();

        yield return new WaitForSeconds(m_dieIntervalTime);
        Destroy(gameObject);
    }

    #endregion

    protected void DetectTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, m_detectRange, LayerMask.GetMask("Player"));

        if (cols == null)
        {
            m_target = null;
            ChangeState(State.IdleState);

        }
        else
        {
            foreach (Collider2D col in cols)
            {
                m_target = col.transform;
                ChangeState(State.ChaseState);
                return;
            }
        }
    }

    protected void DropItem()
    {
        if (m_dropItem == null) return;
        if (m_state != State.DieState) return;

        bool isHeart = (m_dropItem.GetComponent<ItemBase>().ItemType == ItemType.Heart);
        if (isHeart && (Random.Range(0, 3) > 0))
            return;

        GameObject dropItem = GameObject.Instantiate<GameObject>(m_dropItem);

        dropItem.transform.position = transform.position;
        dropItem.GetComponent<Rigidbody2D>().velocity = Vector3.up * 7f;

        GameObject go = GameObject.Find("DropItems");
        if (go == null)
        {
            go = new GameObject() { name = "DropItems" };
        }
        dropItem.transform.SetParent(go.transform);
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
