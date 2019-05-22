using UnityEngine;
using System.Collections;

public class PickupSpawn : MonoBehaviour
{
    GameObject pickup;

    // Use this for initialization
    void Start()
    {

    }

    public bool CanSpawn { get { return pickup == null; } }
    public GameObject Pickup { get { return pickup; } set { pickup = value; } }

    // Update is called once per frame
    void Update()
    {

    }
}
