using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public int Health { get; set; }
    public int MaxHealth { get { return maxHealth; } }
    public bool IsAlive { get { return Health > 0; } }

    private CinemachineBrain brain;
    private CinemachineVirtualCamera currentCamera;
    CinemachineBasicMultiChannelPerlin noise;

    [SerializeField]
    private int maxHealth;

    void Start()
    {
        Health = maxHealth;
        UIManager.Instance.MaxHPUI = Health;
        //Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
        brain = FindAnyObjectByType<CinemachineBrain>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        UIManager.Instance.UpdateUIHP(Health);

        currentCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        noise = currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        StartCoroutine(_ProcessShake(0.2f, 10f, 1f));
        //Debug.Log($"Player takes {damage} damage!");
        //Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }

    public void TakeHealing(int healing)
    {
        Health += healing;
        UIManager.Instance.UpdateUIHP(Health);
        //Debug.Log($"Player takes {healing} healing!");
        //Debug.Log($"PlayerHealth : {Health}/{maxHealth}");
    }

    private IEnumerator _ProcessShake(float duration, float amplitude, float frequency)
    {
        float elapsedTime = 0f;
        float initialAmplitude = noise.m_AmplitudeGain;
        float initialFrequency = noise.m_FrequencyGain;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = initialAmplitude;
        noise.m_FrequencyGain = initialFrequency;
    }
}
