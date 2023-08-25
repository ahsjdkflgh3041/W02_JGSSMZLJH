using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterBase : MonoBehaviour
{
    public enum State
    {
        Idle, Chase, Attack, Hit, Die,
    }
    [SerializeField] protected State m_state;

    SpriteRenderer[] m_renderer;

    protected Transform m_target;
    protected Vector2 m_moveDir = Vector2.one;

    [Header("Status")]
    [SerializeField] protected int m_hp;
    [SerializeField] protected float m_moveSpeed;
    [SerializeField] protected float m_detectRange;
    [SerializeField] protected float m_attackRange;

    protected float m_idleTime = 2f;

    [SerializeField] protected bool m_isAttacking;

    // 수정필요
    protected float m_attackTime = 1;


    private void Start()
    {
        m_state = State.Idle;

        InvokeRepeating(nameof(CheckTarget), 0f, 0.3f);
        StartCoroutine(nameof(StateMachine));
    }

    private void Update()
    {
        
    }

    IEnumerator StateMachine()
    {
        while (m_hp > 0)
        {
            yield return StartCoroutine(m_state.ToString());
        }
    }

    void ChangeState(State _state)
    {
        m_state = _state;
    }

    IEnumerator Idle() 
    {
        Vector3 randVec = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);

        for (float i = 0; i < m_idleTime; i += Time.deltaTime)
        {
            transform.position += randVec * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (randVec.normalized.x < 0 ? -1 : 1);
            transform.localScale = m_moveDir;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Chase()
    {
        Vector3 chaseDir = m_target.transform.position - transform.position;

        if (chaseDir.magnitude > 0.01f)
        {
            transform.position += chaseDir.normalized * m_moveSpeed * Time.deltaTime;
            m_moveDir.x = (chaseDir.normalized.x < 0 ? -1 : 1) ;
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

        m_isAttacking = true;
        Util.ChangeColor(transform, Color.red);

        yield return new WaitForSeconds(m_attackTime);
        Util.ChangeColor(transform, Color.white);
        m_isAttacking = false;
        ChangeState(State.Chase);
    }

    IEnumerator Hit()
    {
        yield return null;
    }

    IEnumerator Die()
    {
        yield return null;
    }

    void CheckTarget()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }
}
