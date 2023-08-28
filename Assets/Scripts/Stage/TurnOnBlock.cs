using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnBlock : MonoBehaviour
{
    private bool isPlayerEnter = false;

    private bool m_isEnemyAllDie = false;

    private int m_enemyCount;

    [SerializeField] private GameObject m_monsterParent;

    [SerializeField] private GameObject m_entryDoor;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerEnter && !m_isEnemyAllDie)
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
        m_enemyCount = m_monsterParent.GetComponentsInChildren<MonsterHealth>().Length;
        //Debug.Log($"Enemy Count : {m_enemyCount}");
        if (m_enemyCount == 0) { m_isEnemyAllDie=true; }        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !m_isEnemyAllDie)
        {
            isPlayerEnter = true;
            m_monsterParent.SetActive(true);
            m_entryDoor.SetActive(true);
            CountEnemy();
        }
    }
}
