using UnityEngine;
using System.Collections;
using System;

public class PickupSpawner : MonoBehaviour
{
    public PickupSpawn[] pickupSpawns;
    public PickupData[] pickups;

    private float spawnTimer;
    private bool isSpawningClient;

    void Start()
    {
        spawnTimer = 4;
    }

    public void Setup(bool isSpawningClient)
    {
        this.isSpawningClient = isSpawningClient;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawningClient)
        {
            return;
        }

        if (spawnTimer <= 0)
        {
            Spawn();
            spawnTimer = UnityEngine.Random.Range(4, 11);
        }
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    public bool PickupSpawned()
    {
        foreach (PickupSpawn spawn in pickupSpawns)
        {
            if (spawn.Pickup != null)
            {
                return true;
            }
        }

        return false;
    }

    int GetFreeSpawnPoint()
    {
        int len = pickupSpawns.Length;
        int choice = 0;

        do
        {
            choice = UnityEngine.Random.Range(0, len--);

            if (pickupSpawns[choice].CanSpawn)
            {
                return choice;
            }
            else
            {
                PickupSpawn temp = pickupSpawns[choice];
                pickupSpawns[choice] = pickupSpawns[pickupSpawns.Length - 1];
                pickupSpawns[pickupSpawns.Length - 1] = temp;
            }
        } 
        while (len > 0);

        return -1;
    }

    void Spawn()
    {
        int totalWeight = 0;

        for (int i = 0; i < pickups.Length; i++)
        {
            totalWeight += (5 - (int)pickups[i].rarity);
        }

        int count = UnityEngine.Random.Range(0, totalWeight);
        int spawn = 0;

        for (int i = 0; i < pickups.Length; i++)
        {
            if (count < (int)pickups[i].rarity)
            {
                spawn = i;
                break;
            }

            totalWeight -= (int)pickups[i].rarity;
        }

        int spawnPoint = GetFreeSpawnPoint();

        if (spawnPoint != -1)
        {
            SetSpawn(spawnPoint, spawn);

            GameObject multiplayerControllerCheck = GameObject.Find("MultiplayerController");
            if (multiplayerControllerCheck)
            {
                multiplayerControllerCheck.GetComponent<MultiplayerController>().SendHostMessage(spawnPoint, spawn);
            }
        }
    }

    public void SetSpawn(int spawnPoint, int pickupValue)
    {
        GameObject pickupType = pickups[pickupValue].pickup;

        PickupSpawn pickupSpawn = pickupSpawns[spawnPoint];
        pickupSpawn.Pickup = (GameObject)Instantiate(pickupType, pickupSpawn.transform.position, pickupSpawn.transform.rotation);

        if (pickupSpawn.Pickup.GetComponentInChildren<Pickup>() != null)
        {
            pickupSpawn.Pickup.GetComponentInChildren<Pickup>().source = GetComponentInChildren<AudioSource>();
        }
    }
}

[Serializable]
public class PickupData
{
    public enum Rarities { Common, Uncommon, Rare, Epic }

    public GameObject pickup;
    public Rarities rarity;
}