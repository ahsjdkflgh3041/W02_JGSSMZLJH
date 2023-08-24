using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static string EnumToString<T>(T _enum)
    {
        return _enum.ToString();
    }

    public static void ChangeColor(Transform _transform, Color _color)
    {
        _transform.GetComponent<SpriteRenderer>().color = _color;
    }

    public static void ChangeColors(Transform _transform, Color _color)
    {
        SpriteRenderer[] sprites;

        sprites = _transform.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite in sprites) 
        {
            sprite.color = _color;
        }
    }
}
