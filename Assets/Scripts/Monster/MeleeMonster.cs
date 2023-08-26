using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeMonster : MonsterBase
{
    GameObject m_meleeAttack;

    protected override void Start()
    { 
        base.Start();

        m_meleeAttack = Utility.GetChild(transform, "Melee Attack").gameObject;
        m_meleeAttack.GetComponent<SpriteRenderer>().color = m_attackColor;
        m_meleeAttack.GetComponent<Melee>().m_meleeDamage = m_attackPower;
        m_meleeAttack.SetActive(false);
    }

    protected override void Update()
    { 
        base.Update();

        if (m_isAttacking)
            m_meleeAttack.SetActive(true);
        else
            m_meleeAttack.SetActive(false);
    }

    protected override void Init()
    {
        base.Init();

        m_meleeAttack.SetActive(false);
    }
}
