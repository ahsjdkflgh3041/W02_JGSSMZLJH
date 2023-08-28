using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRayProjector : MonoBehaviour
{
    BoxCollider2D m_boxCollider;

    private void Awake()
    {
        m_boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float CalculateDistance(Vector2 direction, float distance, LayerMask layer, int ignoreNum, float sizeCoefficient = 0.95f)
    {
        var dashVector = direction * distance;
        var rayStartPoints = GetCorners(sizeCoefficient).Select(c => (Vector2)transform.position + c);

        var distances = rayStartPoints.Select(rsp => {
            var hit = Physics2D.Linecast(rsp, rsp + dashVector, layer);
            var hitPoint = hit ? hit.point : rsp + dashVector;
            var measuredDistance = Vector2.Distance(rsp, hitPoint);
            //Debug.Log($"{rsp}, {hitPoint}");
            Debug.DrawLine(rsp, hitPoint, Color.yellow, 0.1f);
            return Mathf.Min(distance, measuredDistance);
            }
        ).ToList();

        ignoreNum = Mathf.Min(ignoreNum, distances.Count - 1);
        return distances.OrderBy(d => d).ElementAt(ignoreNum);
    }

    public List<Transform> SelectTransforms(Vector2 start, Vector2 end, LayerMask layer, float sizeCoefficient = 0.95f)
    {
        var corners = GetCorners(sizeCoefficient);
        var unionResult = new List<Transform>();
        corners.ForEach(c =>
        {
            var targets = Physics2D.LinecastAll(start + c, end + c, layer).Select(h => h.transform);
            Debug.DrawLine(start + c, end + c, Color.blue, 0.5f);
            unionResult = unionResult.Union(targets).ToList();
        });
        //unionResult.ForEach(t => Debug.Log($"target : {t.name}({t.GetInstanceID()})"));
        return unionResult;
    }

    private List<Vector2> GetCorners(float sizeCoefficient = 0.95f)
    {
        var horizontal = 0.95f * m_boxCollider.size.x * transform.lossyScale.x / 2;
        var vertical = 0.95f * m_boxCollider.size.y * transform.lossyScale.y / 2;

        var upRight = Vector2.right * horizontal + Vector2.up * vertical;
        var upLeft = Vector2.left * horizontal + Vector2.up * vertical;
        var downRight = Vector2.right * horizontal + Vector2.down * vertical;
        var downLeft = Vector2.left * horizontal + Vector2.down * vertical;
        var result = new Vector2[] { upRight, upLeft, downRight, downLeft };
        return result.ToList();

    }
}
