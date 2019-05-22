using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text;

public class AButtonController : MonoBehaviour, IPointerDownHandler 
{

	public PlayerController player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerDown(PointerEventData data)
	{
		player.Jump ();
	}
}
