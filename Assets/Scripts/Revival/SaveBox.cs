using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBox : MonoBehaviour
{
    Transform m_SpawnPoint;

    private void Awake()
    {
        m_SpawnPoint = Utility.GetChild(transform, "Spawn Point");

        if (m_SpawnPoint == null) Debug.Log("child null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RevivalManager.Instance.SaveRevivalPoint(m_SpawnPoint.position);
        }
    }
}
