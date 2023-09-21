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
        activeVirtualCamera = (CinemachineVirtualCamera)CinemachineCore.Instance.GetVirtualCamera(0);
        //activeVirtualCamera.m_Lens.
    }
    #endregion

    #region PrivateMethod
    private void Awake()
    {
    }
    #endregion
}
