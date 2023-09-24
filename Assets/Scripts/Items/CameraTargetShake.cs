using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraTargetShake : MonoBehaviour
{
    #region PublicVariables
    #endregion


    #region PrivateVariables
    [Header("Vibration")]
    [SerializeField] float standardAmplitude;
    [SerializeField] float standardFrequency;
    [SerializeField] float standardDuration;


    List<CameraVibrationInfo> cameraVibrationInfos;
    Vector2 vibrationResult;
    #endregion

    #region PublicMethod

    public void AddCameraVibration(Vector2 _direction, float _amplitude, float _frequency, float _duration)
    {
        cameraVibrationInfos.Add(new CameraVibrationInfo(_direction, _amplitude, _frequency, _duration));
    }
    public void AddCameraVibration(Vector2 _direction)
    {
        AddCameraVibration(_direction, standardAmplitude, standardFrequency, standardDuration);
    }
    #endregion

    #region PrivateMethod
    private void Start()
    {
        cameraVibrationInfos = new();
    }
    void Update()
    {

    if (!vibrationResult.Equals(Vector2.zero))
    {
        transform.Translate(-vibrationResult);
    }

    vibrationResult = Vector2.zero;
    foreach (var v in cameraVibrationInfos.ToList())
    {
        vibrationResult += v.OnUpdate();
        if (v.isEnd)
        {
            cameraVibrationInfos.Remove(v);
        }
    }
    transform.Translate(vibrationResult);

}
    #endregion
}
