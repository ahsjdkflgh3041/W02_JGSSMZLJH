using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region UI_Object
    [SerializeField] private Image m_hpBarImg;
    [SerializeField] private Image m_staminaBarImg;
    [SerializeField] private Image m_smashCoolDownImg;
    #endregion

    #region UI_Use Variable
    [HideInInspector] public float MaxHPUI;
    [HideInInspector] public float MaxStaminaUI;
    [HideInInspector] public float MaxCoolDownUI;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public void UpdateUIAtk(float m_currentStamina, float m_currentSmashCooldown)
    {
        m_staminaBarImg.fillAmount = m_currentStamina / MaxStaminaUI;
        m_smashCoolDownImg.fillAmount = 1.0f - (m_currentSmashCooldown / MaxCoolDownUI);
    }

    public void UpdateUIHP(float  m_currentHP)
    {
        m_hpBarImg.fillAmount = m_currentHP / MaxHPUI;
    }

}
