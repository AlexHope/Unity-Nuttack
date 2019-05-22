using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class PunchSound : MonoBehaviour {

	public AudioClip sound;

	// Use this for initialization
	public IEnumerator Play(){
		audio.Play ();
		yield return new WaitForSeconds(audio.clip.length);
		audio.clip = sound;
		audio.animation.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//public void AddListener()
	//{
		
	//}
}