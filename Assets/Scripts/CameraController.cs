using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform m_player;
    [SerializeField] Vector3 m_offset;
    [SerializeField] float m_chaseSpeed;

    Vector3 m_currentPos;

    private void Start()
    {
        m_currentPos = transform.position;
    }

    private void FixedUpdate()
    {
        m_currentPos = Vector3.Lerp(m_currentPos, m_player.position, Time.deltaTime * m_chaseSpeed) ;
        transform.position = m_currentPos + m_offset;
    }
}
