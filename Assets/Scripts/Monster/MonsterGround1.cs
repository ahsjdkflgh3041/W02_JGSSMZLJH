using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGround1 : MonoBehaviour
{
    bool m_onGround;

    [Header("Ground Collider Settings")]
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float m_groundLength = 0.95f;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 m_groundOffset;
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask m_groundLayer;

    private void Update()
    {
        m_onGround =
            Physics2D.Raycast(transform.position + m_groundOffset, Vector2.down, m_groundLength, m_groundLayer)
            || Physics2D.Raycast(transform.position - m_groundOffset, Vector2.down, m_groundLength, m_groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (m_onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + m_groundOffset, transform.position + m_groundOffset + Vector3.down * m_groundLength);
        Gizmos.DrawLine(transform.position - m_groundOffset, transform.position - m_groundOffset + Vector3.down * m_groundLength);

    }

    public bool GetOnGround() { return m_onGround; }

}
