using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossAttackState : BaseState
{
    private Boss m_boss;


    public BossAttackState(Monster _monster) : base(_monster)
    {
        m_boss = GameObject.Find("Boss").GetComponent<Boss>();
    }

    public override void OnStateStart()
    {
    }

    public override void OnStateUpdate()
    {
        foreach ( BossPattern bossPattern in m_boss.m_patterns.Values)
        {
            bossPattern.WaitCoolTime();

            if (bossPattern.CanAttack == true)
            {
                bossPattern.ExcuteAttack();
                return;
            }
        }
    }

    public override void OnStateExit()
    {
    }
}
