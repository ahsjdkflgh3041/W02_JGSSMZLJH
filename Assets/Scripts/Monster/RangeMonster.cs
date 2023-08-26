using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMonster : MonsterBase
{
    protected override IEnumerator AttackState()
    {

        if (m_target == null)
            yield break;

        yield return StartCoroutine(base.AttackState());
        ShotBullet();
    }

    void ShotBullet()
    {
        GameObject bullet = InstanciatePrefab("Bullet", transform);
        Vector3 targetDir = (m_target.transform.position - transform.position).normalized;
        bullet.GetComponent<Bullet>().m_bulletDir = targetDir;
    }

    GameObject InstanciatePrefab(string _prefab, Transform _parent)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{_prefab}");
        if (prefab == null ) 
            return  null;

        GameObject instance = Instantiate(prefab, _parent.position, prefab.transform.rotation);
        if (prefab == null) 
            return null;

        instance.GetComponent<Bullet>().m_bulletDamage = m_attackPower;

        return instance;
    }

}
