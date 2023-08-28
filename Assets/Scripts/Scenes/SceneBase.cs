using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneBase : MonoBehaviour
{
    public SceneType SceneType { get; protected set; } = SceneType.UnKnown;

    void Awake()
    {
        init();
    }

    protected virtual void init()
    {
        //GameObject go = GameObject.Find("@Scene");
        //if (go == null)
        //{
        //    go = new GameObject() { name = "@Scene" };
        //}

        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null)
        {
            eventSystem = new GameObject() { name = "EventSystem" };
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}
