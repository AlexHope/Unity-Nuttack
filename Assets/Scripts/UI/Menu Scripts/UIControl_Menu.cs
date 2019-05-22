using UnityEngine;
using System.Collections;

public class UIControl_Menu : MonoBehaviour
{
	public virtual void ChangeScene(string sceneName)
	{
		Application.LoadLevel (sceneName);
	}
}
