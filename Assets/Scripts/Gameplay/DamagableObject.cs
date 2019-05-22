using UnityEngine;
using System.Collections;

public class DamagableObject : MonoBehaviour
{
    protected float health;
    protected float maxHealth;
    protected bool isDead;

    protected Shader normalShader;
    protected Shader flashShader;
    protected float shaderCooldown;

    // Use this for initialization
    protected virtual void Start()
    {
        normalShader = Shader.Find("Sprites/Default");
        flashShader = Shader.Find("Sprites/Diffuse Flash");

        health = maxHealth = 6.0f;

        isDead = false;
    }

    public float Health { get { return health; } }
    public float MaxHealth { get { return maxHealth; } }

    public float HealthPercentage { get { return health / maxHealth; } }

    public bool IsDead { get { return isDead; } }

    protected virtual void FlashWhite(bool value)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            if (value)
            {
                rend.material.shader = flashShader;
                rend.material.SetFloat("_FlashAmount", 1);
            }
            else
            {
                rend.material.shader = normalShader;
            }
        }
    }

    public virtual void Hit(PlayerController attacker, int damage)
    {
        health -= damage;
    }

    protected void CheckIfDead()
    {
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            Kill();
        }
    }

    protected virtual void Kill()
    {
        if (shaderCooldown > 0)
        {
            shaderCooldown = 0;
            FlashWhite(false);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CheckIfDead();

        if (shaderCooldown > 0)
        {
            shaderCooldown -= Time.deltaTime;
            if (shaderCooldown < 0)
            {
                shaderCooldown = 0;
                FlashWhite(false);
            }
        } 
    }
}
