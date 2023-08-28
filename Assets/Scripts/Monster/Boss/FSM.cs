using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private BaseState m_currentState;

    public FSM(BaseState _initState)
    {
        m_currentState = _initState;
        ChangeState(m_currentState);
    }

    public void ChangeState(BaseState _nextState)
    {
        if (m_currentState == null) return;

        m_currentState.OnStateExit();
        m_currentState = _nextState;
        m_currentState.OnStateStart();
    }

    public void UpdateState()
    {
        if (m_currentState == null) return;

        m_currentState.OnStateUpdate();
    }
}
