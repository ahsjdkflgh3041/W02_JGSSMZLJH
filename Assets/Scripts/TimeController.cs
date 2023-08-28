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
    [SerializeField] private float m_smashBulletTimeDuration;
    [SerializeField] private float m_additionalBulletTimePerTarget = 0.25f;

    private float m_defalutTimeScale;
    private float m_currentTimeScale;
    private bool m_onBulletTime;
    private Color m_defaultColor;

    public bool OnBulletTime { get { return m_onBulletTime; } }
    public float BulletTimeScale { get { return m_bulletTimeScale; } }

    void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_defaultColor = m_spriteRenderer.color;

        m_defalutTimeScale = 1;
        m_currentTimeScale = m_defalutTimeScale;
    }

    public void StartBulletTime()
    {
        if (!m_onBulletTime)
        {
            m_onBulletTime = true;
            m_currentTimeScale = m_bulletTimeScale;
            ChangeBacklightAlpha(0.5f);
            ApplyTimeScale();
        }
    }

    public void EndBulletTime(int attackedNum = 0)
    {
        if (attackedNum > 0)
        {
            StartCoroutine(EndBulletTimeAfterDelay(m_smashBulletTimeDuration + attackedNum * m_additionalBulletTimePerTarget));
        }
        else
        {
            ExecuteEndBulletTime();
        }
    }
    IEnumerator EndBulletTimeAfterDelay(float delay)
    {
        Debug.Log($"Coroutine with {delay} seconds started. : {Time.realtimeSinceStartup}");
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log($"Bullet time ended. : {Time.realtimeSinceStartup}");
        ExecuteEndBulletTime();
    }
    void ExecuteEndBulletTime()
    {
        if (m_onBulletTime)
        {
            m_onBulletTime = false;
            m_currentTimeScale = m_defalutTimeScale;
            ChangeBacklightAlpha(0);
            ApplyTimeScale();
        }
    }

    public void ApplyDashStiff(List<int> damages)
    {
        StartCoroutine(ApplyDashStiffByDamage(damages));
    }

    IEnumerator ApplyDashStiffByDamage(List<int> damamges)
    {
        //Debug.Log($"Dash stiff starts! : {damamges.Count} damage(s)");
        foreach (var damage in damamges)
        {
            Time.timeScale = m_dashStiffTimeScale;
            yield return new WaitForSecondsRealtime(damage * m_dashStiffPerDamage * (m_onBulletTime ? 0 : 1));
            //Debug.Log($"stiff : {damage} * {m_dashStiffPerDamage}");
            Time.timeScale = m_currentTimeScale;
            yield return new WaitForSecondsRealtime(m_dashStiffDelay * (m_onBulletTime ? 0 : 1));

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
