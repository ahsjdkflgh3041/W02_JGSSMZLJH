using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGround : MonoBehaviour
{
    bool m_onGround;
    bool m_onLeftWall;
    bool m_onRightWall;

    [Header("Ground Collider Settings")]
    [SerializeField] [Tooltip("Length of the ground-checking collider")] private float m_groundLength = 0.95f;
    [SerializeField] [Tooltip("Distance between the ground-checking colliders")] private Vector3 m_groundOffset;
    [SerializeField] [Tooltip("Which layers are read as the ground")] private LayerMask m_groundLayer;

    [Header("Wall Collider Settings")]
    [SerializeField] [Tooltip("Length of the wall-checking collider")] private float m_wallLength = 0.475f;
    [SerializeField] [Tooltip("Distance between the left-checking colliders")] private Vector3 m_wallOffset;
    [SerializeField] [Tooltip("Which layers are read as the wall")] private LayerMask m_wallLayer;

    private void Update()
    {
        m_onGround =
            Physics2D.Raycast(transform.position + m_groundOffset, Vector2.down, m_groundLength, m_groundLayer) 
            || Physics2D.Raycast(transform.position - m_groundOffset, Vector2.down, m_groundLength, m_groundLayer);

        m_onLeftWall =
            Physics2D.Raycast(transform.position + m_wallOffset, Vector2.left, m_wallLength, m_wallLayer)
            || Physics2D.Raycast(transform.position - m_wallOffset, Vector2.left, m_wallLength, m_wallLayer);

        m_onRightWall =
            Physics2D.Raycast(transform.position + m_wallOffset, Vector2.right, m_wallLength, m_wallLayer)
            || Physics2D.Raycast(transform.position - m_wallOffset, Vector2.right, m_wallLength, m_wallLayer);

    }

    private void OnDrawGizmos()
    {
        if (m_onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + m_groundOffset, transform.position + m_groundOffset + Vector3.down * m_groundLength);
        Gizmos.DrawLine(transform.position - m_groundOffset, transform.position - m_groundOffset + Vector3.down * m_groundLength);

        if (m_onLeftWall) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + m_wallOffset, transform.position + m_wallOffset + Vector3.left * m_wallLength);
        Gizmos.DrawLine(transform.position - m_wallOffset, transform.position - m_wallOffset + Vector3.left * m_wallLength);

        if (m_onRightWall) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + m_wallOffset, transform.position + m_wallOffset + Vector3.right * m_wallLength);
        Gizmos.DrawLine(transform.position - m_wallOffset, transform.position - m_wallOffset + Vector3.right * m_wallLength);
    }

    public bool GetOnGround() { return m_onGround; }
    public bool GetOnLeftWall() { return m_onLeftWall; }
    public bool GetOnRightWall() { return m_onRightWall;  }

    public LayerMask GetLayer() 
    {
        return m_groundLayer;  
    }
}
