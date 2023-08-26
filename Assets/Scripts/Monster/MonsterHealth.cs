using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }

    MonsterBase m_monsterBase;
    [SerializeField] int m_maxHealth;

    void Start()
    {
        m_monsterBase = GetComponent<MonsterBase>();

        Health = m_maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            Debug.Log($"{gameObject.name} takes {damage} damage ({Health}/{m_maxHealth}).");

            m_monsterBase.ForceChangeState(State.HitState);
            //if (Health <= 0)
            //{
            //    Debug.Log($"{gameObject.name} died!");
            //    gameObject.SetActive(false);
            //}
        }
    }
}
