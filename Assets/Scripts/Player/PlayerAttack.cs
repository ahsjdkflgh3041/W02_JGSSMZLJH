using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int m_attackPower;
    [SerializeField] private LayerMask m_attackableLayer;
    
    public void Attack(Vector2 start, Vector2 end)
    {
        var attackables = Physics2D.LinecastAll(start, end, m_attackableLayer).Select(a => a.transform.GetComponent<IDamagable>()).ToList();
        foreach (var attackable in attackables)
        {
            attackable.TakeDamage(m_attackPower);
        }
    }
}
