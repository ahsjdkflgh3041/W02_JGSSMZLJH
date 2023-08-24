using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static string EnumToString<T>(T _enum)
    {
        return _enum.ToString();
    }
}
