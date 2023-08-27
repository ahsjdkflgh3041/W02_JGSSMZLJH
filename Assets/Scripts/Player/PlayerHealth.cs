using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }
    public int MaxHealth { get { return maxHealth; } }

    [SerializeField]
    private int maxHealth;

    void Start()
    {
        Health = maxHealth;
        UIManager.Instance.MaxHPUI = Health;
        Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        UIManager.Instance.UpdateUIHP(Health);
        Debug.Log($"Player takes {damage} damage!");
        Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }

    public void TakeHealing(int healing)
    {
        Health += healing;
        UIManager.Instance.UpdateUIHP(Health);
        Debug.Log($"Player takes {healing} healing!");
        Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }
}
