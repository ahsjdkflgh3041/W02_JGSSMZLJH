using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonsterBase
{
    protected override IEnumerator AttackState()
    {
        if (m_target == null)
            yield break;

        if (m_canAttack == true)
        { 
            for (int i = 0; i < 10; i++) 
            {
                ShotBullet();
            }
        }   

        yield return StartCoroutine(base.AttackState());
    }

    void ShotBullet()
    {
        GameObject bullet = Utility.InstanciatePrefab("Bullet", transform);
        bullet.GetComponent<Bullet>().m_bulletDamage = m_attackPower;

        Vector3 targetDir = (m_target.transform.position - transform.position).normalized;
        bullet.GetComponent<Bullet>().m_bulletDir = targetDir;
    }
}
