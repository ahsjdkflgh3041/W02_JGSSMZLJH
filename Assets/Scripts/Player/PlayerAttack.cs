using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private int m_dashPower;
    [SerializeField] private float m_maxStamina;
    [SerializeField] private float m_staminaPerDash;
    [SerializeField] private float m_staminaRegen;
    [SerializeField] private float m_staminaRegenOnGround;
    [Header("Smash")]
    [SerializeField] private int m_smashPower;
    [SerializeField] private float m_smashCooldown;
    [Header("Layer")]
    [SerializeField] private LayerMask m_attackableLayer;

    //private UIManager uiManager;
    private PlayerGround m_ground;
    private PlayerRayProjector m_rayProjector;
    private TimeController m_timeController;

    private float m_currentStamina;
    private float m_currentSmashCooldown;

    private bool m_isAttacking;

    public bool CanDash { get { return m_currentStamina >= m_staminaPerDash; } }
    public bool CanSmash { get { return m_currentSmashCooldown <= 0; } }
    public float CurrentStamina { get { return m_currentStamina; } }

    private void Awake()
    {
        // uiManager = FindObjectOfType<UIManager>();
        m_ground = GetComponent<PlayerGround>();
        m_rayProjector = GetComponent<PlayerRayProjector>();
        m_timeController = FindObjectOfType<TimeController>();

        m_currentStamina = m_maxStamina;
        m_currentSmashCooldown = -1;
    }
    private void Start()
    { 
        UIManager.Instance.MaxStaminaUI = m_maxStamina;
        UIManager.Instance.MaxCoolDownUI = m_smashCooldown;
    }
    private void Update()
    {
        if (m_currentSmashCooldown > 0)
        {
            m_currentSmashCooldown -= Time.deltaTime;
        }
        
        if (m_currentStamina < m_maxStamina)
        {
            m_currentStamina += (m_ground.GetOnGround() || m_ground.GetOnWall()  ? m_staminaRegenOnGround : m_staminaRegen) * Time.deltaTime;
            if (m_currentStamina > m_maxStamina)
            {
                m_currentStamina = m_maxStamina;
            }
        }
        UIManager.Instance.UpdateUIAtk(CanDash, m_currentStamina, m_currentSmashCooldown);
    }

    public void Dash(Vector2 start, Vector2 end)
    {
        m_currentStamina -= m_staminaPerDash;
        PerformAttack(start, end, m_dashPower);
    }

    public void Smash(Vector2 start, Vector2 end)
    {
        m_currentSmashCooldown = m_smashCooldown;
        PerformAttack(start, end, m_smashPower);
    }

    void PerformAttack(Vector2 start, Vector2 end, int damage)
    {
        var attackables = m_rayProjector.SelectTransforms(start, end, m_attackableLayer).Select(a => a.GetComponent<IDamagable>()).ToList();
        var damages = InflictDamages(attackables, damage);
        if (damages.Count > 0)
        {
            m_timeController.ApplyDashStiff(damages);
        }
    }

    List<int> InflictDamages(List<IDamagable> targets, int damage)
    {
        var damages = new List<int>();
        targets.ForEach(a => {
            a.TakeDamage(damage);
            damages.Add(damage);
        });
        return damages;
    }
}
