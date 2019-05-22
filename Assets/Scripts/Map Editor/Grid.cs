#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public float width = 32.0f;
	public float height = 32.0f;
	public Transform tilePrefab;
	public TileSet tileSet;
	public Color colour = Color.white;		// coloUr because we ain't American here
	
	void OnDrawGizmos()
	{
		Vector3 positionCamera = Camera.current.transform.position;
		Gizmos.color = colour;
		
		for (float x = positionCamera.x - 1200.0f; x < positionCamera.x + 1200.0f; x += width)		// Draw the horizontal grid lines
		{
			Gizmos.DrawLine(new Vector3((Mathf.FloorToInt(x/width)*width), -50.0f, 0.0f),
			                new Vector3((Mathf.FloorToInt(x/width)*width), 50.0f, 0.0f));
		}
		
		for (float y = positionCamera.y - 800.0f; y < positionCamera.y + 800.0f; y += height) 		// Draw the vertical grid lines
		{
			Gizmos.DrawLine(new Vector3(-50.0f, (Mathf.FloorToInt(y/height)*height), 0.0f),
			                new Vector3(50.0f, (Mathf.FloorToInt(y/height)*height), 0.0f));
		}
	}
}
#endif