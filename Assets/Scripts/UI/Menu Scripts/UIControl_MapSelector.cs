using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class UIControl_MapSelector : UIControl_Menu {

	private int index;
	private GameObject finalMap;
	private string[] mapList;
	private bool isSelected, isHost;
	private GameObject multiplayerCheck;
	private GameObject cachedSettings;
	public Sprite[] maps;

	// Use this for initialization
	void Start () {
		multiplayerCheck = GameObject.Find ("MultiplayerController");
		if (GameObject.Find ("SingleplayerController")) {
			Destroy (GameObject.Find ("Multiplayer_FadeOverlay"));
			Destroy (GameObject.Find ("SingleplayerController"));
		} else {
			GameObject.Find ("Multiplayer_FadeOverlay").GetComponent<Image> ().enabled = false;
		}

		if (GameObject.Find ("Settings")) {
			cachedSettings = new GameObject ("Cached_Settings");
			cachedSettings.AddComponent<Text>();
			cachedSettings.GetComponent<Text>().text = GameObject.Find ("Settings").GetComponent<Text>().text;
			Destroy (GameObject.Find ("Settings"));
			DontDestroyOnLoad(cachedSettings);
		}

		isSelected = false;
		isHost = false;

		index = 0;
		transform.FindChild ("Map").FindChild ("MapImage").GetComponent<Image> ().overrideSprite = maps [0];
		finalMap = new GameObject ("Final Map");
		finalMap.AddComponent<Text> ();
		mapList = transform.FindChild("Map").GetComponent<UIControl_Settings_Incrementer>().values;
	}
	
	// Update is called once per frame
	void Update () {
		index = transform.FindChild ("Map").GetComponent<UIControl_Settings_Incrementer> ().Index();

		if (!multiplayerCheck) {
			transform.FindChild ("Map").FindChild ("MapImage").GetComponent<Image> ().overrideSprite = maps [index];
		} else if (multiplayerCheck && isHost) {
			transform.FindChild ("Map").FindChild ("MapImage").GetComponent<Image> ().overrideSprite = maps [index];
			multiplayerCheck.GetComponent<MultiplayerController> ().SendMapUpdated (index);
		}
	}

	public void FinalMap()
	{
			if (multiplayerCheck) {
			multiplayerCheck.GetComponent<MultiplayerController>().LoadMultiplayerGame(mapList[index]);
			multiplayerCheck.GetComponent<MultiplayerController>().SendMapMessage(index);
		} else {
			finalMap.GetComponent<Text> ().text = index.ToString ();
			DontDestroyOnLoad (finalMap);
		}
	}

	public override void ChangeScene (string sceneName)
	{
		if (multiplayerCheck && !isSelected) {
			multiplayerCheck.GetComponent<MultiplayerController> ().LeaveRoom ();
			Destroy (multiplayerCheck);
			base.ChangeScene ("mainmenu");
		} else if (multiplayerCheck && isSelected) {isSelected = false;} else {
			base.ChangeScene (sceneName);
		}
	}

	public void Multiplayer_MapChange(List<Participant> participants, Participant currentPlayer)
	{
		if (currentPlayer != participants [0]) {
			GameObject.Find ("Multiplayer_FadeOverlay").GetComponent<Image> ().enabled = true;
			transform.FindChild ("Map").FindChild ("Increase").GetComponent<Button> ().interactable = false;
			transform.FindChild ("Map").FindChild ("Decrease").GetComponent<Button> ().interactable = false;
			transform.FindChild ("OK").GetComponent<Button> ().interactable = false;
			transform.FindChild ("Back").GetComponent<Button> ().interactable = false;
			transform.FindChild ("Wait_For_Host").GetComponent<Text> ().text = "Waiting for host to select a map...";
		} else if (currentPlayer == participants [0]) {
			isHost = true;
			Destroy (GameObject.Find ("Back_Button"));
		}
	}

	public void MapSelected()
	{
		isSelected = true;
	}

	public void UpdateMap(int _index)
	{
		transform.FindChild ("Map").FindChild ("MapImage").GetComponent<Image> ().overrideSprite = maps [_index];
		transform.FindChild ("Map").FindChild ("Value").GetComponent <Text> ().text = mapList [_index];
	}
}
