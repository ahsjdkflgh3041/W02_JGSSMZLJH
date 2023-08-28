using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : BaseState
{
    Boss m_boss;

    float m_destroyTimeCounter;
    float m_destroyTime;

    public DieState(Monster _monster) : base(_monster)
    {
        m_boss = GameObject.Find("Boss").GetComponent<Boss>();

        m_destroyTime = 3;
    }

    public override void OnStateStart()
    {
        Utility.ChangeColor(m_monster.m_renderer, m_monster.m_dieColor);
        m_monster.m_collider.isTrigger = true;

        m_monster.DropItem();
    }

    public override void OnStateUpdate()
    {
        DestoryMonster();
    }

    public override void OnStateExit()
    {
        return;
    }

    void DestoryMonster()
    {
        m_destroyTimeCounter += Time.deltaTime;

        if (m_destroyTimeCounter > m_destroyTime)
        {
            m_boss.m_isDead = true;
        }
    }
}
