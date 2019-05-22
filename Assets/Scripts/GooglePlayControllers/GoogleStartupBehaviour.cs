using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;

public class GoogleStartupBehaviour : MonoBehaviour
{
    private const float FontSizeMult = 0.05f;
    private bool mWaitingForAuth = false;
    private string mStatusText = "Ready.";

    void Start()
    {
        // Select the Google Play Games platform as our social platform implementation
        //GooglePlayGames.PlayGamesPlatform.Activate();

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        // registers a callback to handle game invitations received while the game is not running.
        //.WithInvitationDelegate(<callback method>)
        // registers a callback for turn based match notifications received while the
        // game is not running.
        //.WithMatchDelegate(<callback method>)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);

        // Activate the Google Play Games platform
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        AuthenticateUser();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AuthenticateUser()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log ("Successful Login!");
            }
            else
            {
                Debug.Log ("Failed Login!");
            }
        });
    }
}
