using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class BaseState
{
    protected Monster m_monster;

    protected BaseState(Monster _monster)
    {
        m_monster = _monster;
    }

    public abstract void OnStateStart();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
