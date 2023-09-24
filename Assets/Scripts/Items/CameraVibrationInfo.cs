using UnityEngine;

public class CameraVibrationInfo
{
    Vector2 direction;
    //��鸮�� ����
    float amplitude;
    //���� ȸ��
    float frequency;
    //���� �Ⱓ
    float duration;

    float currentDuration;
    public bool isEnd => currentDuration >= duration;

    public CameraVibrationInfo(Vector2 _direction, float _amplitude, float _frequency, float _duration)
    {
        direction = _direction;
        amplitude = _amplitude;
        frequency = _frequency;
        duration = _duration;

        currentDuration = 0;
    }
    public Vector2 OnUpdate()
    {
        currentDuration += Time.deltaTime;
        return direction.normalized
            * Mathf.Cos(frequency * currentDuration * 2 * Mathf.PI)
            * amplitude * Mathf.Exp(-5 * currentDuration / duration);
    }
}

