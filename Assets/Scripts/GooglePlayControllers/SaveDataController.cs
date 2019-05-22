using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;

public class SaveDataController : MonoBehaviour
{
    private const string filename = "NuttackSaveData";

    private ISavedGameMetadata currentMetaData;
    private bool dataLoaded;

    public int Acorns = 0;

    // Use this for initialization
    void Start()
    {
        dataLoaded = false;

        DontDestroyOnLoad(this);

        OpenSavedData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OpenSavedData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    }

    public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            dataLoaded = true;
            currentMetaData = game;
        }
        else
        {
            // handle error
        }
    }

    public void Save()
    {
        if (!dataLoaded)
        {
            return;
        }

        string stringData = "";

        stringData += Acorns.ToString();

        byte[] data = System.Text.ASCIIEncoding.Default.GetBytes(stringData);

        SaveGame(currentMetaData, data, new TimeSpan());
    }

    void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);
        /*if (savedImage != null)
        {
            // This assumes that savedImage is an instance of Texture2D
            // and that you have already called a function equivalent to
            // getScreenshot() to set savedImage
            // NOTE: see sample definition of getScreenshot() method below
            byte[] pngData = savedImage.EncodeToPNG();
            builder = builder.WithUpdatedPngCoverImage(pngData);
        }*/
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
        }
        else
        {
            // handle error
        }
    }

    public void Load()
    {
        if (!dataLoaded)
        {
            return;
        }

        LoadGameData(currentMetaData);
    }

    void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string stringData = System.Text.ASCIIEncoding.Default.GetString(data);

            Acorns = int.Parse(stringData);
        }
        else
        {
            // handle error
        }
    }
}
