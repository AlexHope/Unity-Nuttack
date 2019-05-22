using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
[CustomEditor(typeof(Grid))]

public class GridEdit : Editor
{
	Grid mapGrid;
	private int oldindex = 0;
	
	void OnEnable()
	{
		mapGrid = (Grid)target;
	}

	[MenuItem("Assets/Create/Tile Set")]		// Adds a 'Tile Set' option to the right click > create
	static void CreateTileSet()
	{
		var asset = ScriptableObject.CreateInstance<TileSet> ();
		var path = AssetDatabase.GetAssetPath (Selection.activeObject);

		if (string.IsNullOrEmpty (path)) 
		{
			path = "Assets";
		} 
		else if (Path.GetExtension(path) != "") 
		{
			path = path.Replace (Path.GetFileName (path), "");
		} 
		else 
		{
			path += "/";
		}

		var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "Tile_Set.asset");
		AssetDatabase.CreateAsset (asset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
	
	public override void OnInspectorGUI()
	{
		mapGrid.width = slider ("Width", mapGrid.width);
		mapGrid.height = slider ("Height", mapGrid.height);
		
		if (GUILayout.Button ("Grid Window")) 
		{
			GridWindow gridWindow = (GridWindow)EditorWindow.GetWindow(typeof(GridWindow));
			gridWindow.init ();
		}
		
		EditorGUI.BeginChangeCheck ();
		var newTilePrefab = (Transform)EditorGUILayout.ObjectField("Tile Prefab", mapGrid.tilePrefab, typeof(Transform), false);
		
		if (EditorGUI.EndChangeCheck ()) 
		{
			mapGrid.tilePrefab = newTilePrefab;
			Undo.RecordObject(target, "Grid Changed");
		}

		EditorGUI.BeginChangeCheck ();
		var newTileSet = (TileSet)EditorGUILayout.ObjectField ("Tile Set", mapGrid.tileSet, typeof(TileSet), false);

		if (EditorGUI.EndChangeCheck ()) 
		{
			mapGrid.tileSet = newTileSet;
			Undo.RecordObject (target, "Grid Changed");
		}

		if (mapGrid.tileSet != null) 
		{
			EditorGUI.BeginChangeCheck();
			var names = new string[mapGrid.tileSet.prefabs.Length];
			var values = new int[names.Length];

			for(int i = 0; i < names.Length; i++)
			{
				names[i] = mapGrid.tileSet.prefabs[i] != null ? mapGrid.tileSet.prefabs[i].name:"";
				values[i] = i;
			}

			var index = EditorGUILayout.IntPopup("Select Tile", oldindex, names, values);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Grid Changed");
				if (oldindex != index)
				{
					oldindex = index;
					mapGrid.tilePrefab = mapGrid.tileSet.prefabs[index];

					float width = mapGrid.tilePrefab.renderer.bounds.size.x;
					mapGrid.width = width;
					float height = mapGrid.tilePrefab.renderer.bounds.size.y;
					mapGrid.height = height;
				}
			}
		}
	}


	private float slider(string label, float position)	// Creates sliders for editor (width/height)
	{
		GUILayout.BeginHorizontal ();
		GUILayout.Label (label + " of Grid");
		position = EditorGUILayout.Slider (position, 0.5f, 100.0f, null);
		GUILayout.EndHorizontal ();
		
		return position;
	}

	Transform GetTransformFromPosition(Vector3 alignment)	// Gimme the tile
	{
		int tempInt = 0;

		while (tempInt < mapGrid.transform.childCount) 
		{
			Transform transform = mapGrid.transform.GetChild (tempInt);

			if (transform.position == alignment)		// Mouse position is over a tile
			{
				return transform;
			}
			else
			{
				tempInt++;
			}
		}
		return null;
	}

	void OnSceneGUI()
	{
		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		Event e = Event.current;
		Ray ray = Camera.current.ScreenPointToRay (new Vector3 (e.mousePosition.x,
		                                                        -e.mousePosition.y + Camera.current.pixelHeight)); // e.mousePosition.y negative because y is inverted
		Vector3 mousePosition = ray.origin;		// z = 0 because 2D

		if (e.type == EventType.MouseDown && e.isMouse && e.button == 0)	// For creating tiles with left mouse button
		{
			GUIUtility.hotControl = controlID;
			e.Use();

			GameObject gameObject;
			Transform prefab = mapGrid.tilePrefab;

			if (prefab)
			{
				Undo.IncrementCurrentGroup();
				Vector3 alignment = new Vector3(Mathf.FloorToInt(mousePosition.x/mapGrid.width)*mapGrid.width + mapGrid.width/2.0f, 
				                                Mathf.FloorToInt(mousePosition.y/mapGrid.height)*mapGrid.height + mapGrid.height/2.0f,
				                                0.0f);

				if (GetTransformFromPosition(alignment) != null)
				{
					return;		// Do not want multiple tiles in same grid square!
				}

				gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab.gameObject);
				gameObject.transform.position = alignment;
				gameObject.transform.parent = mapGrid.transform;		// Objects will be created as child under gameObject
				Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			}
		}

		if (e.type == EventType.MouseDown && e.isMouse && e.button == 1) 	// For deleting tiles with right mouse button
		{
			GUIUtility.hotControl = controlID;
			e.Use();

			Vector3 alignment = new Vector3(Mathf.FloorToInt(mousePosition.x/mapGrid.width)*mapGrid.width + mapGrid.width/2.0f, 
			                                Mathf.FloorToInt(mousePosition.y/mapGrid.height)*mapGrid.height + mapGrid.height/2.0f,
			                                0.0f);
			Transform transform = GetTransformFromPosition(alignment);

			if (transform != null)
			{
				DestroyImmediate(transform.gameObject);		// DELETE THE TILE IF THERE'S A TILE
			}
		}

		if (e.type == EventType.MouseUp && e.isMouse) 
		{
			GUIUtility.hotControl = 0;			// No longer controlling object (cannot drag with mouse)
		}
	}
}