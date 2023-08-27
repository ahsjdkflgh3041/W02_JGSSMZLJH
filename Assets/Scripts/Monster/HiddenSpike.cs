using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSpike : MonoBehaviour
{
    [SerializeField] int m_spikeDamage;
    [SerializeField] float m_coolTime;
    [SerializeField] float m_upInterval;

    Animator m_animator;

    bool m_canAttack = true;

    private void Start()
    {
        m_animator = Utility.GetChild(transform, "Body").GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && m_canAttack == true)
        {
            Invoke(nameof(OnUp), m_upInterval);
        }    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && m_canAttack == true)
        {
            m_canAttack = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_spikeDamage);
            StartCoroutine(nameof(WaitCoolTime));
        }
    }

    private void OnUp()
    {
        if (m_canAttack == false) return;

        m_animator.SetTrigger("onUp");
    }

    IEnumerator WaitCoolTime()
    {
        yield return new WaitForSeconds(m_coolTime);
        m_canAttack = true;
    }
}
