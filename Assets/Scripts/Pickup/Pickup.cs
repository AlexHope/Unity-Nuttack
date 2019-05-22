using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    public AudioSource source;
    public AudioClip pickupSound;
    protected float setupTime = 0;

    public float SetupTime { set { setupTime = value; } }

    protected virtual void Start()
    {
        setupTime = 0.3f;
    }

    protected virtual void CollectPickup(GameObject player)
    {

    }

    protected virtual void Update()
    {
        if (setupTime > 0)
        {
            setupTime -= Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (setupTime <= 0 && col.gameObject.tag == "Player" && !col.GetComponent<PlayerController>().IsDead)
        {
            CollectPickup(col.gameObject);
            Destroy(gameObject);
        }
    }
}
