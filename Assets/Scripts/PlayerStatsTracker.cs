using UnityEngine;
using System.Collections;

public class PlayerStatsTracker : MonoBehaviour {

	public struct PlayerStats
	{
		public int kills;
		public int deaths;
		public int punchesHit;
		public int punchesTotal;
		public int acorns;
	}

	private PlayerController[] players;
	private PlayerStats[] player_stats;

	void Start()
	{
		GameObject[] squirrels = GameObject.FindGameObjectsWithTag("Player");
		players = new PlayerController[squirrels.Length];
		player_stats = new PlayerStats[squirrels.Length];

		for (int i = 0; i < squirrels.Length; i++) 
		{
			players[i] = squirrels[i].GetComponent<PlayerController>();
		}
	}

    public PlayerStats GetPlayerStats(PlayerController player)
    {
        int num = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == player)
            {
                num = i;
                break;
            }
        }

        return player_stats[num];
    }

	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < players.Length; i++)
		{
			player_stats[i].kills = players[i].Score;
			player_stats[i].deaths = players[i].deaths;
			player_stats[i].punchesHit = players[i].punchesHit;
			player_stats[i].punchesTotal = players[i].punchesTotal;
			player_stats[i].acorns = players[i].Acorns;
		}
	}

	public PlayerStats[] PlayerStatsEnd()
	{
		return player_stats;
	}
}
