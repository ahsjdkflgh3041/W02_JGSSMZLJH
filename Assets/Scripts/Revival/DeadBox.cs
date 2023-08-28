using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBox : MonoBehaviour
{
    [SerializeField] int m_fallDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(m_fallDamage);
            
            RevivalManager.Instance.Revival(collision.gameObject);
        }
    }
}
