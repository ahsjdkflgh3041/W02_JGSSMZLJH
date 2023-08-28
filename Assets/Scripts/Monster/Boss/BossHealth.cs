using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class BossHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }
    public bool CanHit { get { return m_canHit; } set { m_canHit = value; } }
    
    public float m_hitCoolTime;

    Boss m_boss;
    Slider m_slider;

    [SerializeField] int m_maxHealth;
    int m_hitCounter;

    bool m_canHit = true;

    public void Start()
    {
        TryGetComponent<Boss>(out m_boss);
        Utility.GetChild(transform, "Slider").gameObject.TryGetComponent<Slider>(out m_slider);

        Health = m_maxHealth;
    }

    private void Update()
    {
        if (Health >= 0)
            m_slider.value = (float)Health / m_maxHealth;
        else
            m_slider.value = 0;
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
