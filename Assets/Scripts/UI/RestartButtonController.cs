using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RestartButtonController : MonoBehaviour, IPointerDownHandler  {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown(PointerEventData data)
	{
		//Destroy(GameObject.Find("Player_Stats"));

		GameObject singleplayerControllerCheck = GameObject.Find("SingleplayerController");
		if (singleplayerControllerCheck)
		{
			Destroy(singleplayerControllerCheck);
		}

		GameObject multiplayerControllerCheck = GameObject.Find("MultiplayerController");
		if (multiplayerControllerCheck)
		{
			multiplayerControllerCheck.GetComponent<MultiplayerController>().LeaveRoom();
			Destroy(multiplayerControllerCheck);
		}

		if (GameObject.Find ("Player_Stats")) {
			Destroy (GameObject.Find ("Player_Stats"));
		}

		Application.LoadLevel("mainmenu");
	}
}
