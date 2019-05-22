#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridWindow : EditorWindow 
{
	Grid mapGrid;
	
	public void init()
	{
		mapGrid = (Grid)FindObjectOfType (typeof(Grid));
	}
	
	void OnGUI()
	{
		mapGrid.colour = EditorGUILayout.ColorField (mapGrid.colour, GUILayout.Width (200));	// For changing the colour of the grid in the window
	}
}
#endif