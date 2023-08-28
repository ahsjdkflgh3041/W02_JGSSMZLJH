using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BossPattern
{
    public bool CanAttack { get { return m_canAttack; } protected set { m_canAttack = value; } }

    protected float m_coolTimeCounter;
    protected float m_coolTime;
    protected bool m_canAttack = true;

    public virtual void WaitCoolTime()
    {
        if (m_canAttack == false)
        {
            m_coolTimeCounter += Time.deltaTime;

            if (m_coolTimeCounter > m_coolTime)
            {
                m_canAttack = true;
                m_coolTimeCounter = 0;
            }
        }
    }

    public abstract void ExcuteAttack();
}

public class BossPattern1 : BossPattern
{
    Transform m_transform;

    int m_summonNum;


    public BossPattern1()
    {
        m_transform = GameObject.Find("Boss").transform;

        m_summonNum = 5;
        m_coolTime = 15;
    }

    public override void ExcuteAttack()
    {
        if (m_canAttack == false) return;
        CanAttack = false;
        
        for (int i = 0; i < m_summonNum; i++) 
        {
            SummonMonster();
        }
        Debug.Log($"pattern1");
    }

    void SummonMonster()
    {
        int randomIndex = Random.Range(0, 2);
        Object prefab;

        if (randomIndex == 0)
            prefab = Resources.Load("Prefabs/Monster/Melee Monster");
        else
            prefab = Resources.Load("Prefabs/Monster/Range Monster");

        if (prefab == null)
            return;

        Vector3 randomPos = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);

        MonsterBase monster = GameObject.Instantiate(prefab, m_transform.position + randomPos, m_transform.rotation).GetComponent<MonsterBase>();
        monster.m_detectRange = 50f;
    }

}

public class BossPattern2 : BossPattern
{
    Transform m_playerTransform;
    Transform m_bossTransform;
    Vector3 m_targetDir;

    float m_shotCoolTime;
    float m_shotCoolTimeCounter;

    int m_bulletNum;
    int m_currentBulletNum;

    public BossPattern2()
    {
        m_coolTime = 30;
        m_shotCoolTime = 1f;
        m_bulletNum = 20;
    }

    public override void ExcuteAttack()
    {
        if (m_canAttack == false) return;

        CanAttack = false;
        m_currentBulletNum = 0;
        m_shotCoolTimeCounter = 0;
    }

    public override void WaitCoolTime()
    {
        base.WaitCoolTime();

        if (m_currentBulletNum == m_bulletNum)
        {
            return;
        }
        DetectTarget();
        m_shotCoolTimeCounter += Time.deltaTime;

        if (m_shotCoolTimeCounter > m_shotCoolTime)
        {
            m_shotCoolTimeCounter -= m_shotCoolTime;
            m_currentBulletNum++;
            ShotBullet();
        }
    }

    void DetectTarget()
    {
        m_bossTransform = GameObject.Find("Boss").transform;
        m_playerTransform = GameObject.Find("Player").transform;

        m_targetDir = (m_playerTransform.position - m_bossTransform.position).normalized;
    }

    void ShotBullet()
    {
        Object prefab = Resources.Load("Prefabs/Monster/Bullet");

        Bullet bullet = GameObject.Instantiate(prefab, m_bossTransform.position, m_bossTransform.rotation).GetComponent<Bullet>();
        bullet.m_bulletDir = m_targetDir.normalized;
        bullet.GetComponent<Transform>().localScale = Vector3.one * 3;
    }
}

public class BossPattern3 : BossPattern
{
    Transform m_playerTransform;
    Transform m_bossTransform;
    Vector3 m_targetDir;

    float m_shotCoolTime;
    float m_shotCoolTimeCounter;

    int m_bulletNum;
    int m_currentBulletNum;
    int m_randomNum;

    public BossPattern3()
    {
        m_coolTime = 20;
        m_shotCoolTime = 0;
        m_bulletNum = 30;
    }

    public override void ExcuteAttack()
    {
        if (m_canAttack == false) return;

        CanAttack = false;
        m_currentBulletNum = 0;
        m_shotCoolTimeCounter = 0;
        m_randomNum = Random.Range(-2, 2);

        DetectTarget();
    }

    public override void WaitCoolTime()
    {
        base.WaitCoolTime();

        if (m_currentBulletNum == m_bulletNum)
        {
            return;
        }
        m_shotCoolTimeCounter += Time.deltaTime;

        if (m_shotCoolTimeCounter > m_shotCoolTime)
        {
            m_shotCoolTimeCounter -= m_shotCoolTime;
            m_currentBulletNum++;
            ShotBullet(m_currentBulletNum);
        }
    }

    void DetectTarget()
    {
        m_bossTransform = GameObject.Find("Boss").transform;
        m_playerTransform = GameObject.Find("Player").transform;

        m_targetDir = (m_playerTransform.position - m_bossTransform.position).normalized;
    }

    void ShotBullet(int _num)
    {
        Object prefab = Resources.Load("Prefabs/Monster/Bullet");
        Bullet bullet = GameObject.Instantiate(prefab, m_bossTransform.position, m_bossTransform.rotation).GetComponent<Bullet>();

        bullet.m_bulletDir = (m_targetDir + _num * Vector3.one * 2).normalized;
        bullet.m_bulletDir = (m_targetDir - _num * Vector3.one * 2).normalized;

        bullet.GetComponent<Transform>().localScale = Vector3.one;
    }
}