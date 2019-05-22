using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerResults : MonoBehaviour {
	
	private Transform player1, player2, player3, player4;
	private Transform[] players;
	private Transform winnerOverlay;
	private int winner;
	private int winnerAnnounced;
	private PlayerStatsTracker scores;

	// Use this for initialization
	void Start () 
    {
		player1 = transform.Find ("player1_scores");
		player2 = transform.Find ("player2_scores");
		player3 = transform.Find ("player3_scores");
		player4 = transform.Find ("player4_scores");
		Transform[] _players = {player1, player2, player3, player4};
		players = _players;

		winner = -1;
		winnerAnnounced = 0;

		//Destroy controllers once game is finished
		/*GameObject controllerDestroy_SP = GameObject.Find ("SingleplayerController");
		GameObject controllerDestroy_MP = GameObject.Find ("MultiplayerController");

		if (controllerDestroy_SP || controllerDestroy_MP) 
		{
			Destroy (controllerDestroy_SP);
			Destroy (controllerDestroy_MP);
		}*/

		//Assigns tracked stats to new GameObject, deletes old one
		GameObject stats = GameObject.Find ("Player_Stats");
		scores = stats.GetComponent<PlayerStatsTracker> ();
		Destroy (stats);

		SetPlayers ();
	}

	public void SetPlayers()
	{
		//Positions of player 1-3 scores 
		Vector2 player1Pos = player1.position;
		Vector2 player2Pos = player2.position;
		Vector2 player3Pos = player3.position;

		//Sets the values for the player scores
		int highscore = 0;
		for (int i = 0; i < scores.PlayerStatsEnd().Length; i++) {

			players[i].GetComponent<Text>().text = "P" + (i + 1);
			players[i].FindChild ("player_deaths").GetComponent<Text>().text = scores.PlayerStatsEnd()[i].deaths.ToString();
			players[i].FindChild ("player_punches_hit").GetComponent<Text>().text = scores.PlayerStatsEnd()[i].punchesHit.ToString ();
			players[i].FindChild ("player_punches_total").GetComponent<Text>().text = scores.PlayerStatsEnd()[i].punchesTotal.ToString();
			players[i].FindChild ("player_acorns").GetComponent<Text>().text = scores.PlayerStatsEnd()[i].acorns.ToString();
			players[i].FindChild ("player_kills").GetComponent<Text>().text = scores.PlayerStatsEnd()[i].kills.ToString ();
			
			if (scores.PlayerStatsEnd()[i].kills > highscore)
			{
				highscore = scores.PlayerStatsEnd()[i].kills;
				winner = i;
			}
			else if (scores.PlayerStatsEnd()[i].kills == highscore)
			{
				winner = -1;
			}
		}

		//Deletes unused player scores GameObjects and attempts to align scores nicely
		for (int i = 4; i > scores.PlayerStatsEnd().Length; i--) {
			if (i == 4)
			{
				Destroy (GameObject.Find("player4_scores"));
				player1Pos.x += 50.0f;
				player2Pos.x += 100.0f;
				player3Pos.x += 150.0f;
				player1.position = player1Pos;
				player2.position = player2Pos;
				player3.position = player3Pos;
			}
			if (i == 3)
			{
				Destroy (GameObject.Find("player3_scores"));
				player1Pos.x += 50.0f;
				player2Pos.x += 100.0f;
				player1.position = player1Pos;
				player2.position = player2Pos;
			}
			// Only run if somehow there is only one player in the game when it ends
			if (i == 2)
			{
				Destroy (GameObject.Find("player2_scores"));
				player1Pos.x += 50.0f;
				player1.position = player1Pos;
			}
		}
	}

	//Called when results screen is clicked on
	public void Continue()
	{
		winnerOverlay = transform.Find ("Win_Overlay_Text");
		Text winText = winnerOverlay.GetComponent<Text> ();

		if (winnerAnnounced == 0) 
		{
			winText.text = "And the Winner is...";
			winnerAnnounced++;
		} 
		else if (winnerAnnounced == 1) 
		{
			if (winner > -1) 
			{
				winText.text = players[winner].GetComponent<Text>().text + "!";
			} 
			else 
			{
				winText.text = "It's a draw!";
			}
			winnerAnnounced++;
		} 
		else if (winnerAnnounced == 2) 
		{
            GameObject singleplayerControllerCheck = GameObject.Find("SingleplayerController");
            if (singleplayerControllerCheck)
            {
                Destroy(singleplayerControllerCheck);
            }

            GameObject multiplayerControllerCheck = GameObject.Find("MultiplayerController");
            if (multiplayerControllerCheck)
            {
                multiplayerControllerCheck.GetComponent<MultiplayerController>().LeaveRoom();
                Destroy(multiplayerControllerCheck);
            }

			Application.LoadLevel("mainmenu");
		}
	}
}
