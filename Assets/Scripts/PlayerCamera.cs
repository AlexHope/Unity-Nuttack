using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

	public GameObject[] sq;
	public RectTransform mapBoundaries;
	public int zoom;
	public int xOffset;
	public int yOffset;
	public float maxZoom = 9;
	public float minZoom = 4;

	private Vector3 tempCamPos;

	// Use this for initialization
	void Start()
	{
		tempCamPos = transform.position;
	}

	Vector3 CapBoundaries(Vector3 camPos)
	{
		if (camera.orthographicSize > mapBoundaries.rect.height / 2)
		{
			camera.orthographicSize = mapBoundaries.rect.height / 2;
		}
		if (camera.orthographicSize > mapBoundaries.rect.width / (2 * camera.aspect))
		{
			camera.orthographicSize = mapBoundaries.rect.width / (2 * camera.aspect);
		}

		/*if (camPos.y < mapBoundaries.rect.min.y)
		{
			camPos.y = mapBoundaries.rect.min.y + camSize;
		}
		else if (camPos.y + camSize > mapBoundaries.rect.max.y)
		{
			camPos.y = mapBoundaries.rect.max.y - camSize;
		}*/

		if (camPos.x < mapBoundaries.rect.min.x + camera.orthographicSize * camera.aspect)
		{
			camPos.x = mapBoundaries.rect.min.x + camera.orthographicSize * camera.aspect;
		}
		if (camPos.x > mapBoundaries.rect.max.x - camera.orthographicSize * camera.aspect)
		{
			camPos.x = mapBoundaries.rect.max.x - camera.orthographicSize * camera.aspect;
		}

		return camPos;
	}

	// Update is called once per frame
	void Update()
	{
		int camFollow = 3;

		for (int i = 0; i < 4; i++)
		{
			if (!sq[i].GetComponent<PlayerController>().IsDead)
			{
				camFollow = i;
				break;
			}
		}

		//calculate high/low points on each axis
		Vector3 oldCamPos = tempCamPos;
		Vector3 newCamPos = tempCamPos;
		float lowX = newCamPos.x;
		float highX = newCamPos.x;
		float lowY = newCamPos.y;
		float highY = newCamPos.y;
		foreach (GameObject i in sq)
		{
			if (i == null || i.GetComponent<PlayerController>().IsDead)
			{
				continue;
			}

			if (i.transform.position.x < lowX) { lowX = i.transform.position.x; }
			else if (i.transform.position.x > highX) { highX = i.transform.position.x; }
			if (i.transform.position.y < lowY) { lowY = i.transform.position.y; }
			else if (i.transform.position.y > highY) { highY = i.transform.position.y; }
		}

		//set camera position without offsets
		newCamPos.x = Mathf.Lerp(highX,lowX,0.5f);
		newCamPos.y = Mathf.Lerp(highY,lowY,0.5f);

		//get largest distance for camera size
		float camSize = 0;
		foreach (GameObject i in sq)
		{
			if (i.GetComponent<PlayerController>().IsDead)
			{
				continue;
			}

			float dist = Vector3.Distance(i.transform.position, newCamPos);
			if (dist + 3 > camSize) { camSize = dist + 3; }
		}

		//set camera size with zoom
		camera.orthographicSize = camSize - zoom;

		//cap
		if (camera.orthographicSize < minZoom)
		{
			camera.orthographicSize = minZoom;
		}
		else if (camera.orthographicSize > maxZoom)
		{
			camera.orthographicSize = maxZoom;
		}

		tempCamPos = newCamPos;

		newCamPos = CapBoundaries(newCamPos);

		//set camera offsets here so it doesn't affect size
		newCamPos.x = newCamPos.x + xOffset;
		newCamPos.y = newCamPos.y + yOffset;

		//tween camera
		camera.transform.position = Vector3.Lerp(oldCamPos,newCamPos,0.1f);
	
	}
}