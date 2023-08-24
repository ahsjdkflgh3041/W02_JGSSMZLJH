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
    [SerializeField] State m_state;

    [SerializeField] Transform m_target;

    [Header("Status")]
    [SerializeField] int m_hp;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_detectRange;

    float m_IdleTime = 2f;
    float m_ChaseTime = 2f;



    private void Start()
    {
        m_state = State.Idle;

        InvokeRepeating(nameof(CheckTarget), 0f, 0.3f);
        StartCoroutine(nameof(StateMachine));
    }

    IEnumerator StateMachine()
    {
        Debug.Log("in StateMachine");

        while (m_hp > 0)
        {
            string str = Util.EnumToString<State>(m_state);
            Debug.Log(str);

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

        for (float i = 0; i < m_IdleTime; i += Time.deltaTime)
        {
            transform.position += randVec * m_moveSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator Chase()
    {
        Vector3 chaseVec =(m_target.transform.position - transform.position).normalized;

        for (float i = 0; i < m_ChaseTime; i += Time.deltaTime)
        {
            transform.position += chaseVec * m_moveSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    IEnumerator Attack()
    {
        yield return null;
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_detectRange);
    }
}
