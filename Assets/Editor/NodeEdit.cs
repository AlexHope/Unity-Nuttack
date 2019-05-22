using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
[CustomEditor(typeof(NodeManager))]

public class NodeEdit : Editor
{
    NodeManager nodeEditor;
    Node selectedNode;
    bool createSinglePath;

    void OnEnable()
    {
        nodeEditor = (NodeManager)target;
        createSinglePath = false;
    }

    Node GetNodeFromPosition(Vector3 position)
    {
        int tempInt = 0;

        while (tempInt < nodeEditor.transform.childCount)
        {
            Transform transform = nodeEditor.transform.GetChild(tempInt);
            Node node = transform.GetComponent<Node>();

            if (node == null)
            {
                return null;
            }

            if (transform.position.x + node.size / 2f >= position.x && transform.position.x - node.size / 2f <= position.x &&
                transform.position.y + node.size / 2f >= position.y && transform.position.y - node.size / 2f <= position.y)		// Mouse position is over a tile
            {
                return node;
            }
            else
            {
                tempInt++;
            }
        }
        return null;
    }

    void ChangeSelectedNode(Node newNode)
    {
        int tempInt = 0;

        while (tempInt < nodeEditor.transform.childCount)
        {
            Transform transform = nodeEditor.transform.GetChild(tempInt);

            transform.GetComponent<Node>().selected = false;

            tempInt++;
        }

        selectedNode = newNode;
        selectedNode.selected = true;
    }

    void OnSceneGUI()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        Event e = Event.current;
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x,
                                                                -e.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePosition = ray.origin;
        mousePosition.z = 0;

        if (e.type == EventType.keyDown)
        {
            GUIUtility.hotControl = controlID;
            e.Use();

            if (Event.current.keyCode == (KeyCode.LeftControl))
            {
                createSinglePath = true;
            }
        }
        if (e.type == EventType.keyUp)
        {
            GUIUtility.hotControl = controlID;
            e.Use();

            if (Event.current.keyCode == (KeyCode.LeftControl))
            {
                createSinglePath = false;
            }
        }

        if (e.type == EventType.MouseDown && e.isMouse && e.button == 0) // Left Click
        {
            GUIUtility.hotControl = controlID;
            e.Use();

            Node selected = GetNodeFromPosition(mousePosition);

            if (selected != null)
            {
                selectedNode.connections.Add(new NodeConnection(selected));

                if (!createSinglePath)
                {
                    selected.connections.Add(new NodeConnection(selectedNode));
                }

                EditorUtility.SetDirty(selected);
            }
            else
            {
                GameObject gameObject = (GameObject)PrefabUtility.InstantiatePrefab(nodeEditor.nodeObject);
                gameObject.transform.position = mousePosition;
                gameObject.transform.parent = nodeEditor.transform;

                if (selectedNode != null)
                {
                    selectedNode.connections.Add(new NodeConnection(gameObject.GetComponent<Node>()));

                    if (!createSinglePath)
                    {
                        gameObject.GetComponent<Node>().connections.Add(new NodeConnection(selectedNode));
                    }
                }
                else
                {
                    ChangeSelectedNode(gameObject.GetComponent<Node>());
                }

                EditorUtility.SetDirty(gameObject);
            }

            EditorUtility.SetDirty(selectedNode);
        }

        if (e.type == EventType.MouseDown && e.isMouse && e.button == 1) 	// Right Click
        {
            GUIUtility.hotControl = controlID;
            e.Use();

            Node selected = GetNodeFromPosition(mousePosition);

            if (selected != null)
            {
                if (selected == selectedNode)
                {
                    selectedNode.selected = false;
                    selectedNode = null;
                }
                else
                {
                    ChangeSelectedNode(selected);
                }
            }
        }

        if (e.type == EventType.MouseUp && e.isMouse)
        {
            GUIUtility.hotControl = 0;			// No longer controlling object (cannot drag with mouse)
        }
    }
}
