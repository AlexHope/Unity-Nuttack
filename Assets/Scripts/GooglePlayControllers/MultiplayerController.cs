using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerController : MonoBehaviour, RealTimeMultiplayerListener
{
	LevelManager levelManager;
	bool startedLevelLoad = false;
	bool gameLoaded = false;
	string[] mapList;

	private byte protocolVersion = 1;
	private List<byte> updateMessage;
	private List<byte> hostMessage;
	private List<byte> mapMessage;

	private int updateMessageLength = 29;
	private int hostMessageLength = 10;
	private int mapMessageLength = 6;

	public void Start()
	{
		updateMessage = new List<byte>(updateMessageLength);
        hostMessage = new List<byte>(hostMessageLength);
		mapMessage = new List<byte> (mapMessageLength);
		mapList = new string[3]{"Grass", "Caves", "Snow"};

		DontDestroyOnLoad(this);

		GameObject singleplayerControllerCheck = GameObject.Find("SingleplayerController");
		if (singleplayerControllerCheck)
		{
			Destroy(singleplayerControllerCheck);
		}
	}

	public void CreateQuickGame()
	{
		/*const int MinOpponents = 1, MaxOpponents = 3;
		const int GameVariant = 0;
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents,
					GameVariant, listener);*/

		const int MinOpponents = 1, MaxOpponents = 3;
		const int GameVariant = 0;
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(MinOpponents, MaxOpponents,
					GameVariant, this);

		Application.LoadLevel("loading");
	}

	public void LoadMultiplayerGame(string _mapName)
	{
		Application.LoadLevel (_mapName);
		startedLevelLoad = true;
	}

	public void JoinFromInbox()
	{
		PlayGamesPlatform.Instance.RealTime.AcceptFromInbox(this);
	}

	public void LeaveRoom()
	{
		PlayGamesPlatform.Instance.RealTime.LeaveRoom();
		gameLoaded = false;
	}

	public void CancelLoad()
	{
		LeaveRoom();
		Application.LoadLevel("mainmenu");
		DestroyObject(this);
	}

	public void SendUpdateMessage(float posX, float posY, float velX, float velY, bool facingLeft, bool attacking, bool blocking, float health, float blockingAmount)
	{
		if (!gameLoaded)
		{
			return;
		}

		updateMessage.Clear();
		updateMessage.Add(protocolVersion);
		updateMessage.Add((byte)'U');
		updateMessage.AddRange(System.BitConverter.GetBytes(posX));
		updateMessage.AddRange(System.BitConverter.GetBytes(posY));
		updateMessage.AddRange(System.BitConverter.GetBytes(velX));
		updateMessage.AddRange(System.BitConverter.GetBytes(velY));
		updateMessage.Add(System.Convert.ToByte(facingLeft));
		updateMessage.Add(System.Convert.ToByte(attacking));
		updateMessage.Add(System.Convert.ToByte(blocking));
		updateMessage.AddRange(System.BitConverter.GetBytes(health));
		updateMessage.AddRange(System.BitConverter.GetBytes(blockingAmount));

		byte[] messageToSend = updateMessage.ToArray();
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(false, messageToSend);
	}

	public void SendHostMessage(int spawnPoint, int pickupValue)
	{
		if (!gameLoaded)
		{
			return;
		}

		hostMessage.Clear();
		hostMessage.Add(protocolVersion);
		hostMessage.Add((byte)'H');
		hostMessage.AddRange(System.BitConverter.GetBytes(spawnPoint));
		hostMessage.AddRange(System.BitConverter.GetBytes(pickupValue));

		byte[] messageToSend = hostMessage.ToArray();
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, messageToSend);
	}
	
	public void SendMapMessage(int _map)
	{
		if (!gameLoaded) 
		{
			return;
		}

		mapMessage.Clear ();
		mapMessage.Add (protocolVersion);
		mapMessage.Add ((byte)'M');
		mapMessage.AddRange (System.BitConverter.GetBytes(_map));

		byte[] messageToSend = mapMessage.ToArray ();
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, messageToSend);
	}

	public void SendMapUpdated(int _map)
	{
		if (!gameLoaded) 
		{
			return;
		}

		mapMessage.Clear ();
		mapMessage.Add (protocolVersion);
		mapMessage.Add ((byte)'S');
		mapMessage.AddRange (System.BitConverter.GetBytes(_map));
		
		byte[] messageToSend = mapMessage.ToArray ();
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll (true, messageToSend);
	}

	public void Update()
	{
		if (startedLevelLoad && !Application.isLoadingLevel)
		{
			if (PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants().Count < 2)
			{
				CancelLoad ();
			}

			startedLevelLoad = false;
			gameLoaded = true;

			levelManager = FindObjectOfType<LevelManager>();
			if (levelManager)
			{
				levelManager.StartMultiplayerGame(PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants(), PlayGamesPlatform.Instance.RealTime.GetSelf());
			}
			else
			{
				UIControl_MapSelector mapSelector = GameObject.Find("Canvas").GetComponent<UIControl_MapSelector>();
				if (mapSelector)
				{
					mapSelector.Multiplayer_MapChange(PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants(), PlayGamesPlatform.Instance.RealTime.GetSelf());
				}
			}
		}

		if (gameLoaded)
		{

		}
	}

	public void OnRoomSetupProgress(float percent)
	{
		throw new System.NotImplementedException();
	}

	public void OnRoomConnected(bool success)
	{
		if (success)
		{
			Application.LoadLevel("mapselector");
			startedLevelLoad = true;
		}
		else
		{
			// Show Failure!
			CancelLoad();
		}
	}

	public void OnLeftRoom()
	{
		//CancelLoad();
		Application.LoadLevel("mainmenu");
	}

	public void OnPeersConnected(string[] participantIds)
	{
		foreach (string participant in participantIds) 
		{
			print ("Player " + participant + " has connected.");
		}
	}

	public void OnPeersDisconnected(string[] participantIds)
	{
		foreach (string participant in participantIds) 
		{
			print ("Player " + participant + " has disconnected.");
		}
	}

	public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
	{
		if (!gameLoaded)
		{
			return;
		}

		byte messageVersion = (byte)data[0];
		char messageType = (char)data[1];

		if (messageType == 'U' && data.Length == updateMessageLength) {
			Dictionary<string, GameObject> players = levelManager.PlayerList ();
			float posX = System.BitConverter.ToSingle (data, 2);
			float posY = System.BitConverter.ToSingle (data, 6);
			float velX = System.BitConverter.ToSingle (data, 10);
			float velY = System.BitConverter.ToSingle (data, 14);
			bool facingLeft = System.Convert.ToBoolean (data [18]);
			bool attacking = System.Convert.ToBoolean (data [19]);
			bool blocking = System.Convert.ToBoolean (data [20]);
			float health = System.BitConverter.ToSingle (data, 21);
			float blockingAmount = System.BitConverter.ToSingle (data, 25);

			players [senderId].GetComponent<PlayerController> ().UpdateMultiplayerData (posX, posY, velX, velY, facingLeft, attacking, blocking, health, blockingAmount);
		} else if (messageType == 'H' && data.Length == hostMessageLength) {
			int spawnPoint = System.BitConverter.ToInt32 (data, 2);
			int pickupValue = System.BitConverter.ToInt32 (data, 6);

			PickupSpawner spawner = levelManager.pickupSpawner;

			spawner.SetSpawn (spawnPoint, pickupValue);
		} else if (messageType == 'M' && data.Length == mapMessageLength) {
			int mapIndex = System.BitConverter.ToInt32(data, 2);
			string mapName = mapList[mapIndex];

			LoadMultiplayerGame(mapName);
		} else if (messageType == 'S' && data.Length == mapMessageLength) {
			int mapIndex = System.BitConverter.ToInt32(data, 2);
			GameObject.Find ("Canvas").GetComponent<UIControl_MapSelector>().UpdateMap(mapIndex);
		}
	}
}
