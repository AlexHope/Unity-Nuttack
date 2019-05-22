using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Text;

public class BButtonController : MonoBehaviour, IPointerDownHandler
{
    public PlayerController player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData data)
    {
        if (player.CanPunch)
        {
            player.Attack();
        }
    }
}
