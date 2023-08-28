using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    int Health { get { return m_health; } }
    [SerializeField] int m_health;

}
