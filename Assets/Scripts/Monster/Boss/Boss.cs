using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss : Monster
{
    public enum State { IdleState, MoveState, BossAttackState, HitState, DieState}
    [SerializeField] private State m_currentState;

    private Dictionary<string, BaseState> m_baseStates = new Dictionary<string, BaseState>();
    public Dictionary<string, BossPattern> m_patterns = new Dictionary<string, BossPattern>();

    [HideInInspector] public GameObject m_target;
    BossHealth m_bossHealth;
    FSM m_fsm;

    public bool m_isDead;

    protected override void Awake()
    {
        base.Awake();
        TryGetComponent<BossHealth>(out m_bossHealth);
    }

    protected override void Start()
    {
        base.Start();

        m_currentState = State.IdleState;
        m_fsm = new FSM(new IdleState(this));

        #region State
        m_baseStates.Add("IdleState", new IdleState(this));
        m_baseStates.Add("MoveState", new MoveState(this));
        m_baseStates.Add("BossAttackState", new BossAttackState(this));
        m_baseStates.Add("HitState", new HitState(this));
        m_baseStates.Add("DieState", new DieState(this));
        #endregion

        #region BossPattern
       // m_patterns.Add("BossPattern1", new BossPattern1());
       // m_patterns.Add("BossPattern2", new BossPattern2());
        m_patterns.Add("BossPattern3", new BossPattern3());
        #endregion
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case State.IdleState:
                if (CanDetectPlayer())
                {
                    if (CanAttackPlayer())
                        ChangeState(State.BossAttackState);
                    else
                        ChangeState(State.MoveState);
                }
                break;
            case State.MoveState:
                if (CanDetectPlayer())
                {
                    if (CanAttackPlayer())
                        ChangeState(State.BossAttackState);
                }
                else
                {
                    ChangeState(State.IdleState);
                }
                break;
            case State.BossAttackState:
                if (CanDetectPlayer())
                {
                    if (!CanAttackPlayer())
                        ChangeState(State.MoveState);
                }
                else
                {
                    ChangeState(State.IdleState);
                }
                break;
            case State.HitState:
                break;
            case State.DieState:
                break;
        }
        m_fsm.UpdateState();

        if (m_bossHealth.Health <= 0)
            ChangeState(State.DieState);

        if (m_isDead)
            Destroy(gameObject);
    }

    public void ChangeState(State _nextState)
    {
        m_currentState = _nextState;

        switch (m_currentState)
        {
            case State.IdleState:
                m_fsm.ChangeState(m_baseStates["IdleState"]);
                break;
            case State.MoveState:
                m_fsm.ChangeState(m_baseStates["MoveState"]);
                break;
            case State.BossAttackState:
                m_fsm.ChangeState(m_baseStates["BossAttackState"]);
                break;
            case State.HitState:
                //m_fsm.ChangeState(m_baseStates["HitState"]);
                StartCoroutine(HitState());
                break;
            case State.DieState:
                m_fsm.ChangeState(m_baseStates["DieState"]);
                break;
        }
    }

    private bool CanDetectPlayer()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, m_detectRange, LayerMask.GetMask("Player"));

        if (cols == null)
        {
            m_target = null;
            return false;
        }
        else
        {
            foreach (Collider2D col in cols)
                m_target = col.gameObject;
                
            return true;
        }
    }

    private bool CanAttackPlayer()
    {
        if (m_target == null) return false;

        float targetDistance = (m_target.transform.position - transform.position).magnitude;
        if (targetDistance < m_attackRange)
            return true;
        else 
            return false;
    }

    protected IEnumerator HitState()
    {
        Utility.ChangeColor(m_renderer, m_hitColor);

        yield return new WaitForSeconds(m_bossHealth.m_hitCoolTime);
        Utility.ChangeColor(m_renderer, m_originColor);
    }
}
