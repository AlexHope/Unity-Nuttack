using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;
using UnityEngine;

public class SingleplayerController : MonoBehaviour
{
    LevelManager levelManager;
    bool startedLevelLoad = false;
	int bots, roundLength;
	bool pickupsToggled, vibrationToggled;

    public void Start()
    {
        DontDestroyOnLoad(this);

        GameObject multiplayerControllerCheck = GameObject.Find("MultiplayerController");
        if (multiplayerControllerCheck)
        {
            Destroy(multiplayerControllerCheck);
        }
    }

    public void StartNewGame(int _bots, bool _pickupsToggled, bool _vibrationToggled, int _roundLength, string _mapName)
    {
		Application.LoadLevel (_mapName);
        startedLevelLoad = true;
		bots = _bots;
		pickupsToggled = _pickupsToggled;
		vibrationToggled = _vibrationToggled;
		roundLength = _roundLength;
    }

    void Update()
    { 
        if (startedLevelLoad && !Application.isLoadingLevel)
        {
            levelManager = FindObjectOfType<LevelManager>();
            levelManager.StartSingleplayerGame(bots, pickupsToggled, vibrationToggled, roundLength);
            startedLevelLoad = false;
        }
    }
}
