using UnityEngine;
using System.Collections;

public class Pickup_Acorn : Pickup
{
    protected override void CollectPickup(GameObject player)
    {
        source.PlayOneShot(pickupSound, 1);

		player.GetComponent<PlayerController> ().Acorns++;
        Destroy(transform.parent.gameObject);
    }
}
