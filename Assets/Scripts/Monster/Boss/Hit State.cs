using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : BaseState
{
    Boss m_boss;

    float m_hitCoolTime;
    float m_hitCoolTimeCounter;

    bool m_isHitting;

    public HitState(Monster _monster) : base(_monster)
    {
        m_boss = GameObject.Find("Boss").GetComponent<Boss>();
        m_hitCoolTime = m_boss.GetComponent<BossHealth>().m_hitCoolTime;
    }

    public override void OnStateStart()
    {
        m_isHitting = true;
        Utility.ChangeColor(m_boss.m_renderer, m_boss.m_hitColor);
    }

    public override void OnStateUpdate()
    {
        WaitHitCoolTime();
    }

    public override void OnStateExit()
    {
        Utility.ChangeColor(m_boss.m_renderer, m_boss.m_originColor);
    }

    void WaitHitCoolTime()
    {
        if (m_isHitting == false) return;

        m_hitCoolTimeCounter += Time.deltaTime;
        if (m_hitCoolTimeCounter > m_hitCoolTime)
        { 
            m_isHitting = false;
            m_hitCoolTimeCounter = 0;
        }
    }

}
