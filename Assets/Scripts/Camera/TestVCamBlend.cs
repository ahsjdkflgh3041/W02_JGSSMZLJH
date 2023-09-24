using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TestVCamBlend : MonoBehaviour
{
    #region PublicVariables
    #endregion

    #region PrivateVariables
    private CinemachineVirtualCamera activeVirtualCamera;
    #endregion

    #region PublicMethod
    public void Expand()
    {
        activeVirtualCamera.Priority = 9;
    }

    public void Reduce()
    {
        Debug.Log("Reduce");
        activeVirtualCamera.Priority = 5;
    }
    #endregion

    #region PrivateMethod
    private void Awake()
    {
        TryGetComponent(out activeVirtualCamera);
    }
    #endregion
}
