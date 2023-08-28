using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }
    [SerializeField] int m_maxHealth;

    public bool CanHit { get { return m_canHit; } set { m_canHit = value; } }


    bool m_canHit = true;
    int m_hitCounter;
    [SerializeField] public float m_hitCoolTime;

    Boss m_boss;

    public void Start()
    {
        TryGetComponent<Boss>(out m_boss);

        Health = m_maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (m_canHit == false)
            return;

        if (Health > 0)
        {
            Health -= damage;
            Debug.Log($"{gameObject.name} takes {damage} damage ({Health}/{m_maxHealth}).");

            m_boss.ChangeState(Boss.State.HitState);
        }
        m_hitCounter++;

        if (m_hitCounter >= 5)
        {
            CanHit = false;
            m_hitCounter = 0;
            StartCoroutine(HitCoolTime());
        }
    }

    IEnumerator HitCoolTime()
    {
        yield return new WaitForSeconds(m_hitCoolTime);
        CanHit = true;
    }

}
