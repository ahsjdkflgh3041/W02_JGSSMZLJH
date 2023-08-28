using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RevivalManager : MonoBehaviour
{
    public static RevivalManager Instance { get { return m_instance; } }
    static RevivalManager m_instance = new RevivalManager();

    public Dictionary<int, Vector3> SavePoints { get { return m_savePoints; } }
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
        InitSavePoints();
       
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
            InitSavePoints();
        }
        _go.transform.position = m_savePoints[m_savePointsIndex];
    }

    private void InitSavePoints()
    {
        m_savePoints.Clear();
        m_savePointsIndex = 0;
        m_savePoints.Add(m_savePointsIndex, m_player.transform.position);
    }
}
