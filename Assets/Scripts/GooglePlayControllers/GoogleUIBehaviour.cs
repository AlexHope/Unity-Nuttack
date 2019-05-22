using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi.Multiplayer;

public class GoogleUIBehaviour : MonoBehaviour
{
    public void ShowAchievementsUI()
    {
        Social.ShowAchievementsUI();
    }

    public void ShowLeaderboardsUI()
    {
        Social.ShowLeaderboardUI();
    }
}
