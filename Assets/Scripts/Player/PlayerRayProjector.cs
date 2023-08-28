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
        var horizontal = 0.95f * m_boxCollider.size.x * transform.lossyScale.x / 2;
        var vertical = 0.95f * m_boxCollider.size.y * transform.lossyScale.y / 2;
        var position = (Vector2)transform.position;
        var dashVector = direction * distance;

        var rayStartPoints = new List<Vector2>();
        var upRightPosition = position + Vector2.right * horizontal + Vector2.up * vertical;
        var upLeftPosition = position + Vector2.left * horizontal + Vector2.up * vertical;
        var downRightPosition = position + Vector2.right * horizontal + Vector2.down * vertical;
        var downLeftPosition = position + Vector2.left * horizontal + Vector2.down * vertical;
        var rayArray = new Vector2[] { upRightPosition, upLeftPosition, downRightPosition, downLeftPosition };
        rayStartPoints = rayArray.ToList();

        var distances = rayStartPoints.Select(rsp => {
            var hit = Physics2D.Linecast(rsp, rsp + dashVector, layer);
            var hitPoint = hit ? hit.point : rsp + dashVector;
            var measuredDistance = Vector2.Distance(rsp, hitPoint);
            Debug.Log($"{rsp}, {hitPoint}");
            Debug.DrawLine(rsp, hitPoint, Color.yellow, 1);
            return Mathf.Min(distance, measuredDistance);
            }
        ).ToList();

        ignoreNum = Mathf.Min(ignoreNum, distances.Count - 1);
        return distances.OrderBy(d => d).ElementAt(ignoreNum);
    }

    private List<RaycastHit2D> ShootRays(Vector2 direction, float distance, LayerMask layer, float sizeCoefficient = 0.95f)
    {
        var horizontal = 0.95f * m_boxCollider.size.x * transform.lossyScale.x / 2;
        var vertical = 0.95f * m_boxCollider.size.y * transform.lossyScale.y / 2;
        var position = (Vector2)transform.position;
        var targetVector = direction * distance;

        var upRightPosition = position + Vector2.right * horizontal + Vector2.up * vertical;
        var upLeftPosition = position + Vector2.left * horizontal + Vector2.up * vertical;
        var downRightPosition = position + Vector2.right * horizontal + Vector2.down * vertical;
        var downLeftPosition = position + Vector2.left * horizontal + Vector2.down * vertical;

        var result = new List<RaycastHit2D>();

        result.Add(Physics2D.Linecast(upRightPosition, upRightPosition + targetVector, layer));
        result.Add(Physics2D.Linecast(upLeftPosition, upLeftPosition + targetVector, layer));
        result.Add(Physics2D.Linecast(downRightPosition, downRightPosition + targetVector, layer));
        result.Add(Physics2D.Linecast(downLeftPosition, downLeftPosition + targetVector, layer));

        return result;
    }
}
