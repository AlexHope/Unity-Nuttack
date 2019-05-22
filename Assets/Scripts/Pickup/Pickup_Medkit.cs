using UnityEngine;
using System.Collections;

public class Pickup_Medkit : Pickup
{
    protected override void CollectPickup(GameObject player)
    {
        source.PlayOneShot(pickupSound, 1);

        player.GetComponent<PlayerController>().ChangeHealth(2);
        Destroy(transform.parent.gameObject);
    }
}
