using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIControl_SingleplayerMenu : UIControl_Menu 
{
	int bots, roundLength;
	string mapName;
	bool pickupsToggled, vibrationToggled;
	GameObject settings;
	int[] settingsIndex;
	
	private GameObject singleplayerGameStart;

	void Start()
	{
		GameObject _map = GameObject.Find ("Final Map");
		settingsIndex = new int[4];
		settings = new GameObject ("Settings");
		mapName = "grass";

		if (_map)
		{
			transform.FindChild ("Map").FindChild("Value").GetComponent<Text>().text = transform.FindChild("Map").GetComponent<UIControl_Settings_Incrementer>().values[int.Parse(_map.GetComponent<Text>().text)];
			mapName = transform.FindChild("Map").FindChild("Value").GetComponent<Text>().text;
			Destroy (_map);
		}

		if (GameObject.Find ("Cached_Settings")) {
			string[] _settings = GameObject.Find("Cached_Settings").GetComponent<Text>().text.Split(',');

			int _bots = int.Parse(_settings[0]);
			int _pickups = int.Parse (_settings[1]);
			int _vibration = int.Parse (_settings[2]);
			int _length = int.Parse (_settings[3]);

			transform.FindChild ("Number of Bots").GetComponent<UIControl_Settings_Incrementer>().SetIndex(_bots);
			transform.FindChild ("Pickups Toggle").GetComponent<UIControl_Settings_Incrementer>().SetIndex(_pickups);
			transform.FindChild ("Vibration Toggle").GetComponent<UIControl_Settings_Incrementer>().SetIndex(_vibration);
			transform.FindChild ("Round Length").GetComponent<UIControl_Settings_Incrementer>().SetIndex(_length);

			Destroy (GameObject.Find ("Cached_Settings"));
		}

		settings.AddComponent<Text> ();
	}

	void Update()
	{
		settings.GetComponent<Text> ().text = "";
		settingsIndex [0] = transform.FindChild ("Number of Bots").GetComponent<UIControl_Settings_Incrementer> ().Index();
		settingsIndex [1] = transform.FindChild ("Pickups Toggle").GetComponent<UIControl_Settings_Incrementer> ().Index();
		settingsIndex [2] = transform.FindChild ("Vibration Toggle").GetComponent<UIControl_Settings_Incrementer> ().Index();
		settingsIndex [3] = transform.FindChild ("Round Length").GetComponent<UIControl_Settings_Incrementer> ().Index();

		for (int i = 0; i < settingsIndex.Length; i++)
		{
			if (i < settingsIndex.Length - 1)
			{
				settings.GetComponent<Text>().text += settingsIndex[i].ToString() + ",";
			}
			else
			{
				settings.GetComponent<Text>().text += settingsIndex[i].ToString();
			}
		}
	}

	public void SingleplayerGame()
	{
		singleplayerGameStart = GameObject.Find ("SingleplayerController");
		
		//Finds number of bots selected
		bots = int.Parse (transform.FindChild ("Number of Bots").FindChild ("Value").GetComponent<Text> ().text);
		
		//Checks if pickups have been toggled
		if (transform.FindChild ("Pickups Toggle").FindChild ("Value").GetComponent<Text> ().text == "Off") {
			pickupsToggled = false;
		} else {
			pickupsToggled = true;
		}
		
		//Checks if vibration has been toggled
		if (transform.FindChild ("Vibration Toggle").FindChild ("Value").GetComponent<Text> ().text == "Off") {
			vibrationToggled = false;
		} else {
			vibrationToggled = true;
		}
		
		//Checks what the round length is set to
		string[] _roundLengthValues = transform.FindChild("Round Length").GetComponent<UIControl_Settings_Incrementer>().values;
		for (int i = 0; i < _roundLengthValues.Length; i++) {
			if (transform.FindChild("Round Length").FindChild ("Value").GetComponent<Text>().text == _roundLengthValues[i]){
				roundLength = i + 1;
			}
		}
		Destroy (settings);
		singleplayerGameStart.GetComponent<SingleplayerController> ().StartNewGame (bots, pickupsToggled, vibrationToggled, roundLength, mapName);
	}

	public void DontDestroySettingsObject()
	{
		DontDestroyOnLoad (settings);
	}
}
