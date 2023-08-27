using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{

    protected Collider2D m_collider2D;
    protected Rigidbody2D m_rigidbody2D;

    [SerializeField] protected ItemType m_itemType;
    public ItemType ItemType { get { return m_itemType; } }

    protected virtual void Start()
    {
        m_collider2D = GetComponent<Collider2D>();
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_collider2D.isTrigger = true;
            //m_rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            m_rigidbody2D.gravityScale = 0f;
        }
    }
}
