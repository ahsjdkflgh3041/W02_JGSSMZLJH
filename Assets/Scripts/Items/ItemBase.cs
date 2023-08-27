using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{

    CircleCollider2D m_collider2D;
    Rigidbody2D m_rigidbody2D;

    public ItemType m_itemType;

    private void Start()
    {
        m_collider2D = GetComponent<CircleCollider2D>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_collider2D.isTrigger = true;
            m_rigidbody2D.bodyType = RigidbodyType2D.Static;
        }
    }
}
