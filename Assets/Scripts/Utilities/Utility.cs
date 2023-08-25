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
}
