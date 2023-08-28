using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    SpriteRenderer m_spriteRenderer;

    [SerializeField] private float m_dashStiffPerDamage;
    [SerializeField] private float m_dashStiffDelay;
    [SerializeField] private float m_dashStiffTimeScale;
    [SerializeField] private float m_bulletTimeScale = 0.05f;

    private float m_defalutTimeScale;
    private float m_currentTimeScale;
    private bool m_onBulletTime;
    private Color m_defaultColor;

    public bool OnBulletTime { get { return m_onBulletTime; } }

    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_defaultColor = m_spriteRenderer.color;

        m_defalutTimeScale = Time.timeScale;
        m_currentTimeScale = m_defalutTimeScale;
    }

    public void StartBulletTime()
    {
        m_onBulletTime = true;
        m_currentTimeScale = m_bulletTimeScale;

        ChangeBacklightAlpha(0.5f);
        ApplyTimeScale();
    }

    public void EndBulletTime()
    {
        m_onBulletTime = false;
        m_currentTimeScale = m_defalutTimeScale;
        ChangeBacklightAlpha(0);
        ApplyTimeScale();
    }

    public void ApplyDashStiff(List<int> damages)
    {
        StartCoroutine(ApplyDashStiffByDamage(damages));
    }

    IEnumerator ApplyDashStiffByDamage(List<int> damamges)
    {
        Debug.Log($"Dash stiff starts! : {damamges.Count} damage(s)");
        foreach (var damage in damamges)
        {
            Time.timeScale = m_dashStiffTimeScale;
            yield return new WaitForSecondsRealtime(damage * m_dashStiffPerDamage);
            Debug.Log($"stiff : {damage} * {m_dashStiffPerDamage}");
            Time.timeScale = m_currentTimeScale;
            yield return new WaitForSecondsRealtime(m_dashStiffDelay);

        }
    }

    void ApplyTimeScale()
    {
        Time.timeScale = m_currentTimeScale;
    }

    void ChangeBacklightAlpha(float alpha)
    {
        var backlight = m_defaultColor;
        backlight.a = alpha;
        m_spriteRenderer.color = backlight;
    }
}
