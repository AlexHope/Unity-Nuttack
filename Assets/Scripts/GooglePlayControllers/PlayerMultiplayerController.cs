using UnityEngine;
using System.Collections;

public class PlayerMultiplayerController : MonoBehaviour
{
    PlayerController player;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePosition(float x, float y)
    {
        if (transform.position.x < x && player.FacingLeft)
        {
            player.Flip();
        }
        else if (transform.position.x > x && !player.FacingLeft)
        {
            player.Flip();
        }

        transform.position = new Vector2(x, y);
    }


}
