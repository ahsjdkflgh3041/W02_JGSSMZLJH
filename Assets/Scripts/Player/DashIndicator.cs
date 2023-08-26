using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashIndicator : MonoBehaviour
{
    [SerializeField] private float m_nearDistance = 1;
    [SerializeField] private float m_farDinstance = 1.5f;

    private PlayerController m_player;
    private LineRenderer m_lineRenderer;
    
    private void Awake()
    {
        m_player = transform.parent.GetComponent<PlayerController>();
        m_lineRenderer = GetComponent<LineRenderer>();    
    }
    void Start()
    {
        
    }

    void Update()
    {
        var position = transform.parent.position;
        var direction = (Vector3)m_player.DashDirection;
        var startPos = position + direction * m_nearDistance;
        var endPos = position + direction * (m_player.OnBulletTime ? m_player.DashDistance : m_farDinstance);

        //var firstQuarter = Vector3.Lerp(startPos, endPos, 0.1f);
        //var thirdQuarter = Vector3.Lerp(startPos, endPos, 0.9f);

        var positions = new Vector3[] { startPos, endPos };
        m_lineRenderer.SetPositions(positions);
    }
}
