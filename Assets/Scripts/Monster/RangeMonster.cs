using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeMonster : MonsterBase
{
    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(base.Attack());

        if (m_target == null)
            yield break;

        GameObject bullet = InstanciatePrefab("Bullet", transform);
        Vector3 targetDir = (m_target.transform.position - transform.position).normalized;
        bullet.GetComponent<Bullet>().m_bulletDir = targetDir;
    }

    GameObject InstanciatePrefab(string _prefab, Transform _parent)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{_prefab}");
        if (prefab == null ) 
            return  null;

        GameObject bullet = Instantiate(prefab, _parent.position, prefab.transform.rotation);
        if (prefab == null) 
            return null; 

        return bullet;
    }

}
