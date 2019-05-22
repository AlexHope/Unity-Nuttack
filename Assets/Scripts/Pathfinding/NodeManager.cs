using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeManager : MonoBehaviour
{
#if UNITY_EDITOR
    public GameObject nodeObject;
#endif

    public NodeConnection GetClosestNodeTo(Vector2 position)
    {
        float minimum = 100000;
        Node returnValue = null;

        foreach (Node node in GetComponentsInChildren<Node>())
        {
            if (!node.canBeDestination)
            {
                continue;
            }

            float distance = Vector2.Distance(position, node.transform.position);
            if (distance < minimum)
            {
                minimum = distance;
                returnValue = node;
            }
        }

        return new NodeConnection(returnValue);
    }

    private float CalculateHeuristicCost(Node first, Node second)
    {
        return Vector2.Distance(first.transform.position, second.transform.position);
    }

    private Stack<NodeConnection> BuildPath(Dictionary<NodeConnection, NodeConnection> navPath, NodeConnection current)
    {
        Stack<NodeConnection> path = new Stack<NodeConnection>();
        path.Push(current);

        while (navPath.ContainsKey(current))
        {
            current = navPath[current];
            path.Push(current);
        }

        return path;
    }

    public Stack<NodeConnection> FindPath(NodeConnection start, NodeConnection goal)
    {
        HashSet<NodeConnection> closed = new HashSet<NodeConnection>();
        HashSet<NodeConnection> open = new HashSet<NodeConnection>();
        Dictionary<NodeConnection, NodeConnection> navPath = new Dictionary<NodeConnection, NodeConnection>();

        Dictionary<NodeConnection, float> gScore, fScore;
        gScore = new Dictionary<NodeConnection, float>();
        fScore = new Dictionary<NodeConnection, float>();

        open.Add(start);
        //navPath.Push(start);

        gScore[start] = 0;
        fScore[start] = gScore[start] + CalculateHeuristicCost(start.node, goal.node);

        while (open.Count > 0)
        {
            NodeConnection current = null;
            float lowestFScore = 100000;

            foreach (NodeConnection node in open)
            {
                if (fScore[node] < lowestFScore)
                {
                    lowestFScore = fScore[node];
                    current = node;
                }
            }

            if (current.node == goal.node)
            {
                return BuildPath(navPath, current);
            }

            open.Remove(current);
            closed.Add(current);

            foreach (NodeConnection neighbour in current.node.connections)
            {
                if (closed.Contains(neighbour))
                {
                    continue;
                }

                float g = gScore[current] + Vector2.Distance(current.node.transform.position, neighbour.node.transform.position);

                if (!open.Contains(neighbour) || g < gScore[neighbour])
                {
                    navPath[neighbour] = current;

                    gScore[neighbour] = g;
                    fScore[neighbour] = g + CalculateHeuristicCost(neighbour.node, goal.node);

                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        foreach (Node node in GetComponentsInChildren<Node>())
        {
            for (int i = 0; i < node.connections.Count; i++)
            {
                if (node.connections[i] == null)
                {
                    node.connections.RemoveAt(i);
                    i--;
                }
            }

            Gizmos.color = Color.white;
            foreach (NodeConnection connection in node.connections)
            {
                Gizmos.DrawLine(node.transform.position, connection.node.transform.position);
            }
        }

        foreach (Node node in GetComponentsInChildren<Node>())
        {
            if (node.canBeDestination)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.black;
            }
            Gizmos.DrawCube(node.transform.position, new Vector3(node.size, node.size, node.size));

            if (node.selected)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawCube(node.transform.position, new Vector3(node.size / 2f, node.size / 2f, node.size / 2f));
            }
        }
    }
#endif
}