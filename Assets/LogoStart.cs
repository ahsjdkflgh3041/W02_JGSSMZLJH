using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoStart : MonoBehaviour
{
    [SerializeField ]private GameObject m_text_Dash;
    [SerializeField] private GameObject m_text_Rush;
    [SerializeField] private GameObject m_startBtn;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroText());
    }

    IEnumerator IntroText()
    {
        yield return new WaitForSecondsRealtime(6f);
        m_text_Dash.SetActive(true);
        yield return new WaitForSecondsRealtime(0.6f);
        m_text_Rush.SetActive(true);
        yield return new WaitForSecondsRealtime(0.6f);
        m_startBtn.SetActive(true);
    }
}
