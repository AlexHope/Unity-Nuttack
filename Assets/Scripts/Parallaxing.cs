using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour 
{
	public Transform[] backgrounds;			// Array of backgrounds and foregrounds for parallaxing
	public float smoothing;					// Parallax smoothing - set above 0 or it breaks

	private float[] parallaxingScales;		// Proportion of the camera's movement to move the backgrounds by
	private Transform cameraTransform;				// Main camera transform
	private Vector3 previousCamPosition;	// Position of camera in previous frame

	// Happens before initialization
	void Awake()
	{
		//camera = Camera.main.transform;
	}

	// Use this for initialization
	void Start () 
	{
		cameraTransform = Camera.main.transform;
		previousCamPosition = cameraTransform.position;

		parallaxingScales = new float[backgrounds.Length];

		for (int i = 0; i < backgrounds.Length; i++) 
		{
			parallaxingScales[i] = (backgrounds[i].position.z * -1);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < backgrounds.Length; i++) 
		{
			float parallax = (previousCamPosition.x - cameraTransform.position.x) * parallaxingScales[i];

			// Apply parallax to a position
			float backgroundTargetPositionX = backgrounds[i].position.x + parallax;
			Vector3 backgroundTargetPosition = new Vector3 (backgroundTargetPositionX,
			                                                backgrounds[i].position.y,
			                                                backgrounds[i].position.z);
			backgrounds[i].position = Vector3.Lerp (backgrounds[i].position, backgroundTargetPosition, (smoothing * Time.deltaTime));
		}

		previousCamPosition = cameraTransform.position;
	}
}
