using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : ItemBase
{
    [SerializeField] int m_healingPower;

    protected override void Start()
    {
        base.Start();

        m_itemType = ItemType.Heart;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            OnUse(playerHealth);
        }
    }

    public override void OnUse(PlayerHealth playerHealth)
    {
        if (playerHealth != null && playerHealth.Health < playerHealth.MaxHealth)
        {
            playerHealth.TakeHealing(m_healingPower);
            Destroy(gameObject);
        }
    }
}
