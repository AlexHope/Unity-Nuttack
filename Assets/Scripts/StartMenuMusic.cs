using UnityEngine;
using System.Collections;

public class StartMenuMusic : MonoBehaviour {
	void Awake () {
		DontDestroyOnLoad (transform.gameObject);
	}
}
