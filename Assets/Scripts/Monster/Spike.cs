using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] int m_spikeDamage;
    [SerializeField] float m_coolTime;

    bool m_canAttack = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && m_canAttack == true)
        {
            m_canAttack = false;
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_spikeDamage);
            StartCoroutine(nameof(WaitCoolTime));
        }
    }

    IEnumerator WaitCoolTime()
    {
        yield return new WaitForSeconds(m_coolTime);
        m_canAttack = true;
    }
}
