using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TurnOnBlock : MonoBehaviour
{
    private bool m_isPlayerEnter = false;

    private bool m_isEnemyAllDie = false;

    private int m_enemyCount;

    [SerializeField] private GameObject m_monsterParent;

    [SerializeField] private GameObject m_entryDoor;

    [SerializeField] private CinemachineVirtualCamera mapCamera;

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayerEnter && !m_isEnemyAllDie)
        {
            CountEnemy();
        }
        else
        {
            m_monsterParent.SetActive(false);
            m_entryDoor.SetActive(false);
        }
    }

    private void CountEnemy()
    {
        m_enemyCount = m_monsterParent.GetComponentsInChildren<MonsterMove>().Length;

        if (m_enemyCount == 0) { m_isEnemyAllDie = true; }

        
        UIManager.Instance.UpdateUIEnemy(m_enemyCount);

        if (m_enemyCount == 0) 
        { 
            m_isEnemyAllDie = true;
            UIManager.Instance.UpdateUIEnemyFirst(m_enemyCount, false);
            if (mapCamera != null)
            {
                mapCamera.Priority = 0;
            }
        }        

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !m_isEnemyAllDie)
        {
            Debug.Log("LevelTriggerOn");
            m_isPlayerEnter = true;
            m_monsterParent.SetActive(true);
            m_entryDoor.SetActive(true);
            Collider2D collider2D = gameObject.GetComponent<Collider2D>();
            collider2D.enabled = false;

            m_enemyCount = m_monsterParent.GetComponentsInChildren<MonsterMove>().Length;
            UIManager.Instance.UpdateUIEnemyFirst(m_enemyCount, m_isPlayerEnter);
            CountEnemy();

            if (mapCamera != null)
            {
                mapCamera.Priority = 10;
            }
        }
    }
}
