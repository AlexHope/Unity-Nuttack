using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class NodeConnection
{
    public enum Rules { Walk, Jump, WallJump }

    public Node node;
    public Rules rule;

    public NodeConnection(Node n)
    {
        node = n;
        rule = Rules.Walk;
    }
}
