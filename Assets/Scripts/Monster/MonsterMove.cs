using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    public Vector3 MoveDir { get; set; } = Vector3.one;

    MonsterGround m_ground;
    Vector3 m_localScale;

    [SerializeField] private float m_moveSpeed;

    public void Start()
    {
        m_ground = GetComponent<MonsterGround>();
        m_localScale = GetComponent<Transform>().localScale;
    }

    protected void LateUpdate()
    {
        if (m_ground.GetOnGround() == true)
            MoveDir = Turn(MoveDir);
    }

    public void Move()
    {
        transform.position += MoveDir.normalized * m_moveSpeed * Time.deltaTime;
        transform.localScale = new Vector3((MoveDir.x < 0 ? m_localScale.x * -1 : m_localScale.x), m_localScale.y, m_localScale.z);
    }

    public Vector3 Turn(Vector3 _moveDir)
    {
        Debug.Log($"{gameObject.name} turn");
        return new Vector3(_moveDir.x * -1, _moveDir.y * -1, _moveDir.z);
    }
}
