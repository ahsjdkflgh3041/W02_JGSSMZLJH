using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public Vector3 MoveDir { get; set; } = Vector3.one;

    MonsterGround m_ground;
    Transform m_transform;
    Vector3 m_localScale;

    [SerializeField] private float m_moveSpeed;

    public void Awake()
    {
        TryGetComponent<MonsterGround>(out m_ground);
        TryGetComponent<Transform>(out m_transform);
        m_localScale = m_transform.localScale;
    }

    public void Move()
    {
        transform.position += MoveDir.normalized * m_moveSpeed * Time.deltaTime;
        transform.localScale = new Vector3((MoveDir.x < 0 ? m_localScale.x * -1 : m_localScale.x), m_localScale.y, m_localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("turn");
            MoveDir = Vector2.Reflect(MoveDir, collision.contacts[0].normal);
        }
    }
}
