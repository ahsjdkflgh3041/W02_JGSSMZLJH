using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer m_renderer;
    [HideInInspector] public Collider2D m_collider;
    [HideInInspector] public MonsterHealth m_health;
    [HideInInspector] public MonsterMove m_move;

    [Header("Color")]
    [HideInInspector] public Color m_originColor;
    [HideInInspector] public Color m_attackColor;
    [HideInInspector] public Color m_hitColor;
    [HideInInspector] public Color m_dieColor;

    //[Header("Time")]
    //public float m_dieIntervalTime = 0.5f;

    [Header("Drop Item")]
    [SerializeField] GameObject m_dropItem;

    [Header("Range")]
    [SerializeField] protected float m_detectRange;
    [SerializeField] protected float m_attackRange;

    protected virtual void Awake()
    {
        m_renderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        m_health = GetComponent<MonsterHealth>();
        m_move = GetComponent<MonsterMove>();
    }

    protected virtual void Start()
    {
        m_originColor = m_renderer.color;
        m_attackColor = new Color(255 / 255f, 122 / 255f, 0 / 255f, 255 / 255f);
        m_hitColor = Color.red;
        m_dieColor = new Color(m_originColor.r, m_originColor.g, m_originColor.b, 60 / 255f);
    }

    public void DropItem()
    {
        if (m_dropItem == null) return;

        bool isHeart = (m_dropItem.GetComponent<ItemBase>().ItemType == ItemType.Heart);
        if (isHeart && (Random.Range(0, 3) > 0))
            return;

        GameObject dropItem = GameObject.Instantiate<GameObject>(m_dropItem);

        dropItem.transform.position = transform.position;
        dropItem.GetComponent<Rigidbody2D>().velocity = Vector3.up * 7f;

        GameObject go = GameObject.Find("DropItems");
        if (go == null)
        {
            go = new GameObject() { name = "DropItems" };
        }
        dropItem.transform.SetParent(go.transform);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_attackRange);
    }
}
