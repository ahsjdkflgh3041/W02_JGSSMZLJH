using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : SceneBase
{
    public static GameScene Instance { get { return s_instance; } }
    private static GameScene s_instance = new GameScene();

    public GameObject m_gameOverCanvas;
    GameObject m_player;

    private bool m_isGameOver;

    void Awake()
    {
        base.init();

        if (s_instance == null) { s_instance = this; }

        SceneType = SceneType.Game;

        m_gameOverCanvas = GameObject.Find("GameOver Canvas");
        m_gameOverCanvas.SetActive(false);

        m_player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        m_isGameOver = IsGameOver(m_player);
        if (m_isGameOver)
        {
            m_gameOverCanvas.SetActive(true);
        }    

        if (m_isGameOver && Input.anyKey)
        { 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            RevivalManager.Instance.InitSavePoints();
        }
    }

    public void LoadClearScene()
    {
        SceneManager.LoadScene(2);
    }

    private bool IsGameOver(GameObject _player)
    {
        int playerHealth = _player.GetComponent<PlayerHealth>().Health;

        if (playerHealth <= 0)
            return true;
        else
            return false;
    }
}
