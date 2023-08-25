using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHealth : MonoBehaviour, IDamagable
{
    [SerializeField] int m_maxHealth;
    public int Health { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        Health = m_maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (Health > 0)
        {
            Health -= damage;
            Debug.Log($"{gameObject.name} takes {damage} damage ({Health}/{m_maxHealth}).");
            if (Health <= 0)
            {
                Debug.Log($"{gameObject.name} died!");
                gameObject.SetActive(false);
            }
        }
    }
}
