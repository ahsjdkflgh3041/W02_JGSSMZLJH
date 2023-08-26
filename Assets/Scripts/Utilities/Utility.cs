using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static string EnumToString<T>(T _enum)
    {
        return _enum.ToString();
    }

    public static void ChangeColor(SpriteRenderer _spriteRenderer, Color _color)
    {
        _spriteRenderer.color = _color;
    }

    public static void ChangeColors(SpriteRenderer[] renderers, Color _color)
    {
        foreach(var spriteRenderer in renderers) 
        {
            spriteRenderer.color = _color;
        }
    }

    public static Transform GetChild(Transform _parent, string _name)
    {
        Transform[] _childs = _parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in _childs)
        {
            if (child.gameObject.name == _name)
                return child;
        }
        return null;
    }

    public static GameObject InstanciatePrefab(string _prefab, Transform _parent)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{_prefab}");
        if (prefab == null)
            return null;

        GameObject instance = Instantiate(prefab, _parent.position, prefab.transform.rotation);
        if (prefab == null)
            return null;

        return instance;
    }
}
