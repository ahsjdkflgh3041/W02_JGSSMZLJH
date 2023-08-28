using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RevivalManager : MonoBehaviour
{
    public static RevivalManager Instance { get; private set; }
    public Dictionary<int, Vector3> SavePoint { get { return m_savePoints; } private set { } }
    static Dictionary<int, Vector3> m_savePoints = new Dictionary<int, Vector3>();

    GameObject m_player;
    
    int m_savePointsIndex;

    bool m_isGameOver;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        m_savePointsIndex = 0;
        m_savePoints.Add(m_savePointsIndex, m_player.transform.position);
    }

    public void SaveRevivalPoint(Vector3 _revivalPos)
    {
        Vector3 newRevivalPoint = _revivalPos;
        m_savePoints.Add(++m_savePointsIndex, newRevivalPoint);
    }

    public void Revival(GameObject _go)
    {
        if (m_savePoints == null)
            _go.transform.position = Vector3.zero;

        if (m_isGameOver == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            _go.transform.position = m_savePoints[0];

            return;
        }
        else
        {
            _go.transform.position = m_savePoints[m_savePointsIndex];
        }
    }
}
