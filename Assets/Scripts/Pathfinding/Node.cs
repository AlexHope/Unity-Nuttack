using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
#if UNITY_EDITOR
    [HideInInspector]
    [NonSerialized]
    public bool selected = false;
    [HideInInspector]
    public float size = 0.3f;
#endif

    public bool canBeDestination = true;

    public List<NodeConnection> connections;
}
