using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MoveState : BaseState
{
    GameObject m_target;
    MonsterMove m_monsterMove;

    public MoveState(Monster _monster) : base(_monster)
    {
    }

    public override void OnStateStart()
    {
        m_target = m_monster.GetComponent<Boss>().m_target;
        m_monsterMove = m_monster.GetComponent<MonsterMove>();
    }

    public override void OnStateUpdate()
    {
        m_monsterMove.MoveDir = (m_target.transform.position - m_monsterMove.transform.position).normalized;
        m_monsterMove.Move();
    }

    public override void OnStateExit()
    {
        return;
    }
}
