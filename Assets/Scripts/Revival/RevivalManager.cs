using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RevivalManager : MonoBehaviour
{
    public static RevivalManager Instance { get { return s_instance; } }
    static RevivalManager s_instance = new RevivalManager();

    public Dictionary<int, Vector3> SavePoints { get { return m_savePoints; } }
    static Dictionary<int, Vector3> m_savePoints = new Dictionary<int, Vector3>();

    GameObject m_player;
    Vector3 m_startPoing;
    
    int m_savePointsIndex;

    private void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        m_startPoing = m_player.transform.position;

        InitSavePoints();
    }

    public void SaveRevivalPoint(Vector3 _revivalPos)
    {
        Vector3 newRevivalPoint = _revivalPos;
        m_savePoints.Add(++m_savePointsIndex, newRevivalPoint);
    }

    public void Revival(GameObject _player)
    {
        if (m_savePoints == null)
            _player.transform.position = Vector3.zero;

        _player.transform.position = m_savePoints[m_savePointsIndex];
    }

    public void InitSavePoints()
    {
        m_savePoints.Clear();
        m_savePointsIndex = 0;
        m_savePoints.Add(m_savePointsIndex, m_startPoing);
    }
}
