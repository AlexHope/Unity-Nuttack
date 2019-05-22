using UnityEngine;
using System.Collections;

public class UIControl_StartScreen : UIControl_Menu
{
	GoogleStartupBehaviour startup;

	public void Start()
	{
		startup = GetComponent<GoogleStartupBehaviour>();
	}

	public override void ChangeScene(string sceneName)
	{
		//startup.AutheticateUser();

		if (Social.localUser.authenticated)
		{
            //SaveDataController saveData = GameObject.FindObjectOfType<SaveDataController>();
            //saveData.Load();

			base.ChangeScene(sceneName);
		}
		else
		{
			startup.AuthenticateUser();
		}
	}
}
