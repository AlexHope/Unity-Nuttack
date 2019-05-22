using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class LevelManager : MonoBehaviour
{
    public enum GameProgress { None, Countdown, Start, InGame }

    public GameObject player;
    public Canvas uiCanvas;
    public GameObject eventSystem;
    public Camera playerCamera;
    public RectTransform mapBoundaries;
    public NodeManager nodeManager;
    public PickupSpawner pickupSpawner;

    private GameProgress progress;
    private GameObject[] players;
    private GameObject playerStats;
    private Text startCountdown;
    private Text gameTimer;
    private float timer;
	private int roundLength;

    public GameObject healthBarPrefab;
    public float barOffsetX = 0;
    public float barOffsetY = 0;

    private Canvas ui;
    private Camera cam;
    private List<Participant> participants;
    private Participant currentPlayer;
    private Dictionary<string, GameObject> playerDictionary;

    // Use this for initialization
    void Start()
    {
        ui = (Canvas)Instantiate(uiCanvas);
        cam = (Camera)Instantiate(playerCamera);

        cam.GetComponent<PlayerCamera>().mapBoundaries = mapBoundaries;
        playerStats = new GameObject("Player_Stats");
        DontDestroyOnLoad(GameObject.Find("Player_Stats"));
		roundLength = 60;
		
		progress = GameProgress.None;

        //StartSingleplayerGame(3, true, true, 10);
    }

    public void StartSingleplayerGame(int bots, bool pickupsToggled, bool vibrationToggled, int _roundLength)
    {
        //Health bar variables
        RectTransform healthBarRectTransform = healthBarPrefab.GetComponent<RectTransform>();
        RectTransform containerRectTransform = ui.GetComponent<RectTransform>();

        players = new GameObject[bots + 1];
        cam.GetComponent<PlayerCamera>().sq = new GameObject[bots + 1];
		roundLength = _roundLength * 10;

        playerStats.AddComponent<PlayerStatsTracker>();

		pickupSpawner.Setup(true);
        List<GameObject> spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
        for (int i = 0; i < bots + 1; i++)
        {
            GameObject point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            spawnPoints.Remove(point);
            players[i] = (GameObject)Instantiate(player, point.transform.position, Quaternion.identity);

            foreach (SpriteRenderer part in players[i].GetComponentsInChildren<SpriteRenderer>())
            {
                part.sortingOrder += 40 - (i * 10);
            }

            cam.GetComponent<PlayerCamera>().sq[i] = players[i];

            //Tag for multiplayer:
            /*Text tag = ui.transform.FindChild("PlayerNum" + (i + 1)).GetComponent<Text>();
            tag.GetComponent<PlayerNumTag>().SetupTag(i);*/

            Text tag = ui.transform.FindChild("PlayerNum" + (i + 1)).GetComponent<Text>();
            tag.GetComponent<PlayerNumTag>().playerTransform = players[i].transform;

            //Health Bar
            GameObject newBar = Instantiate(healthBarPrefab) as GameObject;
            newBar.name = "HealthBar " + (i + 1);
            newBar.transform.SetParent(ui.transform);

            if (i == 0 && Social.localUser.authenticated)
            {
                players[i].GetComponentInChildren<HealthBarController>().userImage = Social.localUser.image;
            }

            players[i].GetComponentInChildren<HealthBarController>().InitHealth(newBar, i);

            //position bar
            RectTransform newBarRectTransform = newBar.GetComponent<RectTransform>();
            newBarRectTransform.localScale = new Vector3(1, 1, 1);

            float barWidth = healthBarRectTransform.rect.width;
            float barHeight = healthBarRectTransform.rect.height;
            float xPos;
            float yPos;

            xPos = healthBarRectTransform.rect.width * i + (healthBarRectTransform.rect.width*0.5f);
			yPos = -healthBarRectTransform.rect.height - (healthBarRectTransform.rect.height*0.5f);
			newBarRectTransform.offsetMin = new Vector2(xPos, yPos);

            xPos = newBarRectTransform.offsetMin.x + healthBarRectTransform.rect.width;
            yPos = newBarRectTransform.offsetMin.y + healthBarRectTransform.rect.height;
            newBarRectTransform.offsetMax = new Vector2(xPos, yPos);

            //RectTransform barTextRectTransform = newBar.transform.Find("Text").GetComponent<RectTransform>();

            if (i == 0)
            {
                tag.GetComponent<PlayerNumTag>().SetupTag(i);
                players[i].GetComponent<PlayerController>().isPrimaryPlayer = true;
				if (!vibrationToggled)
				{
					players[i].GetComponent<PlayerController>().vibrationToggled = false;
				}
				else
				{
					players[i].GetComponent<PlayerController>().vibrationToggled = true;
				}
                continue;
            }

            tag.GetComponent<PlayerNumTag>().SetupTag(-1);

            players[i].AddComponent<AISquirrel>();

            switch (Random.Range(0, bots + 1))
            {
                case 0: players[i].GetComponent<ReskinableAnimation>().ChangeBaseSprite("squirrelSkinRed");
                    break;
                case 1: players[i].GetComponent<ReskinableAnimation>().ChangeBaseSprite("squirrelSkinWhite");
                    break;
                case 2: players[i].GetComponent<ReskinableAnimation>().ChangeBaseSprite("squirrelSkinBrown");
                    break;
            }
        }

        PlayerController playerControl = players[0].GetComponent<PlayerController>();

        ui.GetComponentInChildren<DPadController>().player = playerControl;
        ui.GetComponentInChildren<AButtonController>().player = playerControl;
        ui.GetComponentInChildren<BButtonController>().player = playerControl;

        startCountdown = ui.transform.FindChild("StartCountdown").GetComponent<Text>();
        gameTimer = ui.transform.FindChild("Timer").GetComponent<Text>();

        for (int i = 1; i < bots + 1; i++)
        {
            players[i].GetComponent<AISquirrel>().enemies = new List<Transform>();
            players[i].GetComponent<AISquirrel>().nodeManager = nodeManager;
            players[i].GetComponent<AISquirrel>().pickupSpawner = pickupSpawner;

            for (int j = 0; j < bots + 1; j++)
            {
                if (j != i)
                {
                    players[i].GetComponent<AISquirrel>().enemies.Add(players[j].GetComponent<Transform>());
                }
            }
        }

		if (!pickupsToggled) {
			Destroy (pickupSpawner);
			Destroy (GameObject.Find("PickupSpawner"));
		}

        progress = GameProgress.Countdown;
        timer = 3;
    }

    public void StartMultiplayerGame(List<Participant> participants, Participant currentPlayer)
    {
        this.participants = participants;
        this.currentPlayer = currentPlayer;

		if (participants[0] == currentPlayer)
        {
            pickupSpawner.Setup(true);
        }
        playerDictionary = new Dictionary<string, GameObject>();

        //Health bar variables
        RectTransform healthBarRectTransform = healthBarPrefab.GetComponent<RectTransform>();
        RectTransform containerRectTransform = ui.GetComponent<RectTransform>();

        int numOfPlayers = participants.Count;

        players = new GameObject[numOfPlayers];
        cam.GetComponent<PlayerCamera>().sq = new GameObject[numOfPlayers];
        PlayerController playerControl = null;

        //GameObject playerStats = new GameObject("Player_Stats");
        playerStats.AddComponent<PlayerStatsTracker>();

        List<GameObject> spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
        for (int i = 0; i < numOfPlayers; i++)
        {
            GameObject point = spawnPoints[i];
            players[i] = (GameObject)Instantiate(player, point.transform.position, Quaternion.identity);

            playerDictionary[participants[i].ParticipantId] = players[i];

            foreach (SpriteRenderer part in players[i].GetComponentsInChildren<SpriteRenderer>())
            {
                part.sortingOrder += 40 - (i * 10);
            }

            cam.GetComponent<PlayerCamera>().sq[i] = players[i];

            //Tag for multiplayer:
            /*Text tag = ui.transform.FindChild("PlayerNum" + (i + 1)).GetComponent<Text>();
            tag.GetComponent<PlayerNumTag>().SetupTag(i);*/

            Text tag = ui.transform.FindChild("PlayerNum" + (i + 1)).GetComponent<Text>();
            tag.GetComponent<PlayerNumTag>().playerTransform = players[i].transform;
            tag.GetComponent<PlayerNumTag>().SetupTag(i);

			//////////////
			//Health Bar//
			//////////////
			GameObject newBar = Instantiate(healthBarPrefab) as GameObject;
			newBar.name = "HealthBar " + (i + 1);
			newBar.transform.SetParent(ui.transform);

			//get user image
			//should work solely with code in else{}
            if (participants[i] == currentPlayer && Social.localUser.authenticated)
            {
                players[i].GetComponentInChildren<HealthBarController>().userImage = Social.localUser.image;
            }
			else if (participants[i].Player != null)
			{
				if (participants[i].Player.AvatarURL != "")
				{
					WWW urlImage = new WWW(participants[i].Player.AvatarURL);
					urlImage.LoadImageIntoTexture(players[i].GetComponentInChildren<HealthBarController>().userImage);
				}
			}

			//initialize bar
			players[i].GetComponentInChildren<HealthBarController>().InitHealth(newBar, i);
			
			//position bar
			RectTransform newBarRectTransform = newBar.GetComponent<RectTransform>();
			newBarRectTransform.localScale = new Vector3(1, 1, 1);
			
			float barWidth = healthBarRectTransform.rect.width;
			float barHeight = healthBarRectTransform.rect.height;
			float xPos;
			float yPos;
			
			xPos = healthBarRectTransform.rect.width * i + (healthBarRectTransform.rect.width*0.5f);
			yPos = -healthBarRectTransform.rect.height - (healthBarRectTransform.rect.height*0.5f);
			newBarRectTransform.offsetMin = new Vector2(xPos, yPos);
			
			xPos = newBarRectTransform.offsetMin.x + healthBarRectTransform.rect.width;
			yPos = newBarRectTransform.offsetMin.y + healthBarRectTransform.rect.height;
			newBarRectTransform.offsetMax = new Vector2(xPos, yPos);

            //RectTransform barTextRectTransform = newBar.transform.Find("Text").GetComponent<RectTransform>();

            if (participants[i].ParticipantId == currentPlayer.ParticipantId)
            {
                playerControl = players[i].GetComponent<PlayerController>();
                playerControl.isPrimaryPlayer = true;
                playerControl.isMultiplayer = true;
                playerControl.multiplayerController = GameObject.Find("MultiplayerController").GetComponent<MultiplayerController>();
                continue;
            }

            players[i].AddComponent<PlayerMultiplayerController>();
        }

        if (playerControl != null)
        {
            ui.GetComponentInChildren<DPadController>().player = playerControl;
            ui.GetComponentInChildren<AButtonController>().player = playerControl;
            ui.GetComponentInChildren<BButtonController>().player = playerControl;
        }

        startCountdown = ui.transform.FindChild("StartCountdown").GetComponent<Text>();
        gameTimer = ui.transform.FindChild("Timer").GetComponent<Text>();

        progress = GameProgress.Countdown;
        timer = 3;

        //roundLength = 10;
    }

    string FormatTime(float time)
    {
        int d = (int)(time * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateCountdown()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (Mathf.RoundToInt(timer) == 0)
            {
                startCountdown.text = "Fight!";
                timer = 0;
                progress = GameProgress.Start;

                startCountdown.CrossFadeAlpha(0, 1, false);
            }
            else
            {
                startCountdown.text = string.Format("{0:0}", timer);
            }
        }
    }

    void UpdateGameStart()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().HasActiveControl = true;
        }

        progress = GameProgress.InGame;

        timer = 3 * roundLength;
    }

    void UpdateInGame()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            gameTimer.text = FormatTime(timer);

            if (Mathf.RoundToInt(timer) == 0)
            {
                int mostKills = 0;

                PlayerController currentPlayer = players[0].GetComponent<PlayerController>();

                foreach (GameObject player in players)
                {
                    PlayerController playerControl = player.GetComponent<PlayerController>();

                    if (playerControl.isPrimaryPlayer)
                    {
                        currentPlayer = playerControl;

                        //SaveDataController saveData = GameObject.FindObjectOfType<SaveDataController>();

                        //saveData.Acorns += currentPlayer.Acorns;
                        //saveData.Save();
                    }

                    playerControl.HasActiveControl = false;
                    playerControl.StopRespawning();

                    if (mostKills < playerControl.kills)
                    {
                        mostKills = playerControl.kills;
                    }
                }

                PlayerStatsTracker.PlayerStats current = playerStats.GetComponent<PlayerStatsTracker>().GetPlayerStats(currentPlayer);

                int acorns = current.acorns;

                PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQBQ", acorns, (bool success) => { });
                PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQBg", acorns, (bool success) => { });
                PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQBw", acorns, (bool success) => { });
                PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQCA", acorns, (bool success) => { });

                if (currentPlayer.kills == mostKills)
                {
                    PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQAQ", 1, (bool success) => { });
                    PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQAg", 1, (bool success) => { });
                    PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQAw", 1, (bool success) => { });
                    PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQBA", 1, (bool success) => { });

                    if (players.Length == 4)
                    {
                        if (current.punchesHit == current.punchesTotal)
                        {
                            PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQCQ", 1, (bool success) => { });
                        }

                        int kills = 0;

                        for (int i = 0; i < players.Length; i++)
                        {
                            PlayerStatsTracker.PlayerStats stat = playerStats.GetComponent<PlayerStatsTracker>().GetPlayerStats(players[i].GetComponent<PlayerController>());

                            kills += stat.kills;
                        }

                        if (kills > 0 && kills == current.kills)
                        {
                            PlayGamesPlatform.Instance.IncrementAchievement("CgkIxc3w-uEMEAIQCw", 1, (bool success) => { });
                        }
                    }
                }

                timer = 0;

                startCountdown.CrossFadeAlpha(1, 1, false);

                Application.LoadLevel("scorescreen");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (progress)
        {
            case GameProgress.Countdown: UpdateCountdown();
                break;
            case GameProgress.Start: UpdateGameStart();
                break;
            case GameProgress.InGame: UpdateInGame();
                break;
        }
    }

    public Dictionary<string, GameObject> PlayerList()
    {
        return playerDictionary;
    }

    void OnGUI()
    {
        /*GUI.contentColor = Color.red;

        GUI.Label(new Rect(0, 0, 500, 100), "Num of Players: " + participants.Count);
        GUI.Label(new Rect(0, 15, 500, 100), "Self: " + currentPlayer.ParticipantId);

        int next = 0;
        foreach (Participant p in participants)
        {
            GUI.Label(new Rect(0, 30 + next * 15, 500, 100), "P" + (next + 1) + ": " + p.ParticipantId);
            next++;
        }*/
    }
}
