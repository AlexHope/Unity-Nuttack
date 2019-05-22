using UnityEngine;
using System.Collections;

public class Pickup_AcornCrate : DamagableObject
{
    public GameObject acorn;

    private int minNoOfAcorns = 5;
    private int maxNoOfAcorns = 20;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        health = maxHealth = 5;
    }

    public override void Hit(PlayerController attacker, int damage)
    {
        if (isDead)
        {
            return;
        }

        float knockback = 10;

        FlashWhite(true);
        shaderCooldown = 0.1f;

        base.Hit(attacker, damage);

        Vector2 vel = rigidbody2D.velocity;

        vel.x = knockback * -attacker.transform.localScale.x;

        rigidbody2D.velocity = vel;
    }

    protected override void Kill()
    {
        base.Kill();

        for (int i = 0; i < Random.Range(minNoOfAcorns, maxNoOfAcorns); i++ )
        {
            GameObject a = (GameObject)Instantiate(acorn, transform.position, Quaternion.identity);

            Vector3 vel = new Vector3(Random.Range(-4, 5), Random.Range(4, 6), 1);

            a.rigidbody2D.velocity = vel;
            a.rigidbody2D.rotation = Random.Range(0, 360);
            a.GetComponentInChildren<Pickup>().source = GameObject.FindObjectOfType<PickupSpawner>().GetComponent<AudioSource>();
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
