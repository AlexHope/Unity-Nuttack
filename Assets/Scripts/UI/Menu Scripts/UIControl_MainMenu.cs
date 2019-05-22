using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIControl_MainMenu : UIControl_Menu 
{
	void Start()
	{
		GameObject singleplayerController = GameObject.Find ("SingleplayerController");
		GameObject multiplayerController = GameObject.Find ("MultiplayerController");

		if (singleplayerController) {
			Destroy (singleplayerController);
		}
		if (multiplayerController) {
			Destroy (multiplayerController);
		}
	}
}