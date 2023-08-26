using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }

    [SerializeField]
    private int maxHealth;

    void Start()
    {
        Health = maxHealth;
        Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"Player takes {damage} damage!");
        Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }
}
