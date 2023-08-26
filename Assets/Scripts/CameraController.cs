using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [Header("Setting")]
    [SerializeField] Vector3 m_offset;
    [SerializeField] float m_chaseSpeed;
    [SerializeField] float m_smashSizeChangeSpeed;
    [SerializeField] float m_originSizeChangeSpeed;
    [SerializeField] float m_smashCameraSize;


    Transform m_player;
    PlayerController m_playerController;

    Vector3 m_currentPos;

    float m_originCameraSize;
    float m_currentCameraSize;

    private void Start()
    {
        m_player = GameObject.FindWithTag("Player").transform;
        m_playerController = m_player.GetComponent<PlayerController>();

        m_currentPos = transform.position;
        m_originCameraSize = Camera.main.orthographicSize;
        m_currentCameraSize = m_originCameraSize;
    }

    private void FixedUpdate()
    {
        m_currentPos = Vector3.Lerp(m_currentPos, m_player.position, Time.deltaTime * m_chaseSpeed);
        transform.position = m_currentPos + m_offset;

        if (m_playerController.OnBulletTime == true)
            Camera.main.orthographicSize = Mathf.Lerp(m_currentCameraSize, m_smashCameraSize, Time.deltaTime * m_smashSizeChangeSpeed);
        else
            Camera.main.orthographicSize = Mathf.Lerp(m_currentCameraSize, m_originCameraSize, Time.deltaTime * m_originSizeChangeSpeed);
        m_currentCameraSize = Camera.main.orthographicSize;
    }
}
