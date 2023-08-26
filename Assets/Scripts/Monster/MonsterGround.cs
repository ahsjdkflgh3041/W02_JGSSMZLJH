using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGround : MonoBehaviour
{
    bool m_onGround;

    [Header("Ground Collider Settings")]
    [SerializeField][Tooltip("Radius of the ground-checking collider")] private float m_groundRadius = 0.95f;
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask m_groundLayer;

    private void Update()
    {
        m_onGround = Physics2D.OverlapCircle(transform.position, m_groundRadius, m_groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawWireSphere(transform.position, m_groundRadius);
    }

    public bool GetOnGround() { return m_onGround; }

}
