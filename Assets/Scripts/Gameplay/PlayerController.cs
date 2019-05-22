using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class PlayerController : DamagableObject
{
    private int score;
    private int acorns;

    public Animator anim;
    public float speed;
    public float acceleration;
    public GameObject shield;
    private int moveDirection = 0;
    private bool hasActiveControl = false;
    public bool isPrimaryPlayer;
    public int kills;
    public int deaths;
    public int punchesHit;
    public int punchesTotal;

    AudioSource source;
    public AudioClip punchSound;
    public AudioClip hitSound1;
    public AudioClip hitSound2;
    public AudioClip hitSound3;

    private Vector2 amountToMove;
    private float currentSpeed;
    private float targetSpeed;
    private bool facingLeft = true;

    private int jumpCount = 0;
    public float jumpSpeed; //Check how high the player can jump
    public int numberOfJumps;
    private bool wallJump = false;
    private Vector2 wallJumpContact = new Vector2();
    private int contactPoints = 0;

    private bool attacking = false;
    private bool attackingUp = false;
    public float punchDistance;
    private float punchDelay = 0.1f;
    private float punchCounter = 0;
    private bool beingHit = false;
    private bool beingBounced = false;
    private bool aimingUp = false;

    private bool blocking = false;
    private float blockingAmount = 2;
    private float blockingCounter = 0;
    private float blockingDelayAmount = 1;
    private float blockingDelayCounter = 0;

    private int respawnPhase;
    private float respawnTimer;
    private bool invulnerable = false;
    private float invulnerableTimer;
    private float fadeTime;
    private bool fadeOut;

	public bool vibrationToggled;
    public bool isMultiplayer = false;
    public MultiplayerController multiplayerController;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        //if (GameObject.Find ("MultiplayerController")) {
        //	isMultiplayer = true;
        //}

        kills = 0;
        deaths = 0;
        punchesHit = 0;
        punchesTotal = 0;

        vibrationToggled = true;
        blockingCounter = blockingAmount;
        Score = 0; //score test
        Acorns = 0; //acorns test

        source = GetComponent<AudioSource>();
    }

    public int MoveDirection { set { moveDirection = value; } }
    public bool AimingUp { set { aimingUp = value; } }
    public bool Blocking { get { return blocking; } set { blocking = value; } }
    public bool CanBlock { get { return blockingCounter > 0 && blockingDelayCounter == 0 && hasActiveControl && !wallJump && jumpCount == 0; } }
    public bool CanPunch { get { return punchCounter == 0 && !blocking && !beingHit && !attacking; } }
    public int JumpCount { get { return jumpCount; } }
    public bool FacingLeft { get { return facingLeft; } }
    public bool HasActiveControl { get { return hasActiveControl; } set { hasActiveControl = value; } }
    public bool Invulnerable { get { return invulnerable; } }
    public float BlockingCounter { get { return blockingCounter; } }
    public float BlockingAmount { get { return blockingAmount; } }
    public int Score { get { return score; } set { score = value; } }
    public int Acorns { get { return acorns; } set { acorns = value; } }

    public void EndBeingHit() { beingHit = false; }
    public void EndAttacking()
    {
        attacking = false;
        attackingUp = false;
    }
    public void StopRespawning() { respawnPhase = -1; }

    public void ChangeHealth(int value)
    {
        health += value;

        if (health <= 0)
        {
            deaths++;
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Jump()
    {
        if (!hasActiveControl)
        {
            return;
        }

        Vector2 vel = rigidbody2D.velocity;

        if (jumpCount < numberOfJumps)
        {
            vel.y = jumpSpeed;

            if (wallJump)
            {
                vel.x += (jumpSpeed * wallJumpContact.x) / 2;
                //Flip();
            }

            rigidbody2D.velocity = vel;
            jumpCount++;
            wallJump = false;
        }
    }

    public void Attack()
    {
        if (!hasActiveControl || invulnerable)
        {
            return;
        }

        attacking = true;
        if (aimingUp)
        {
            attackingUp = true;
        }
    }

    public void CausePunch()
    {
        punchCounter = punchDelay;

        if (beingHit)
        {
            return;
        }

        //Punch animation has finished, check for contact and act.
        Vector2 dir;
        float dist = punchDistance;

        punchesTotal++;

        if (attackingUp)
        {
            dir = new Vector2(0, 1);
            dist = punchDistance * 1.8f;
        }
        else if (facingLeft)
        {
            dir = new Vector2(-1, 0);
        }
        else
        {
            dir = new Vector2(1, 0);
        }

        Vector2 startPos = transform.position;
        startPos.x -= 0.1f * (dir.x);

        source.PlayOneShot(punchSound);

        RaycastHit2D[] results = Physics2D.RaycastAll(startPos, dir, dist, LayerMask.GetMask("Players") + LayerMask.GetMask("PunchableObject")); //Physics2D.CircleCast(startPos, 0.5f, dir, dist, LayerMask.GetMask("Players"));
        Debug.DrawLine(startPos, startPos + dir * (dist), Color.red, 2);
        foreach (RaycastHit2D result in results)
        {
            if (result.collider.gameObject != null && result.collider.gameObject != this.gameObject && !result.collider.gameObject.GetComponent<DamagableObject>().IsDead)
            {
                punchesHit++;
                result.collider.gameObject.GetComponent<DamagableObject>().Hit(this, 1);
                //break;
            }
        }
    }

    protected override void FlashWhite(bool value)
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

    public override void Hit(PlayerController attacker, int damage)
    {
        if (isDead || invulnerable)
        {
            return;
        }

        float knockback = 12;

        EndAttacking();

        if (blocking)
        {
            blockingCounter = 0;
            blockingDelayCounter = blockingDelayAmount;
            knockback /= 2;
        }
        else
        {
            int rand = Random.Range(0, 3);

            switch (rand)
            {
                case 0: source.PlayOneShot(hitSound1);
                    break;
                case 1: source.PlayOneShot(hitSound2);
                    break;
                case 2: source.PlayOneShot(hitSound3);
                    break;
            }

            if (isPrimaryPlayer && vibrationToggled)
            {
                Vibration.Vibrate(150);
            }

            FlashWhite(true);
            shaderCooldown = 0.1f;

            base.Hit(attacker, damage);
            beingHit = true;

            if (health <= 0)
            {
                attacker.kills++;
                deaths++;
            }
        }

        Vector2 vel = rigidbody2D.velocity;

        vel.x = knockback * -attacker.transform.localScale.x;

        rigidbody2D.velocity = vel;
    }

    public void TakeDamage(int damage)
    {
        if (isPrimaryPlayer && vibrationToggled)
        {
            Vibration.Vibrate(150);
        }

        FlashWhite(true);
        shaderCooldown = 0.1f;

        beingHit = true;
        ChangeHealth(damage);
    }

    public void Bounce()
    {
        beingBounced = true;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("IsHanging", wallJump);

        float xVelocity = rigidbody2D.velocity.x;
        bool run = (xVelocity < -2f || xVelocity > 1f) && xVelocity != 0 && jumpCount == 0 && !wallJump;

        anim.SetBool("IsHit", beingHit);
        anim.SetBool("IsRunning", run);
        anim.SetBool("IsJumping", jumpCount > 0 || wallJump);
        anim.SetBool("IsPunching", attacking && !attackingUp && !beingHit);
        anim.SetBool("IsPunchingUp", attacking && attackingUp && !beingHit);
        anim.SetBool("IsBouncing", beingBounced);
        anim.SetBool("IsBlocking", blocking);
    }

    private void UpdateMovement()
    {
        if (!hasActiveControl)
        {
            return;
        }

        Vector2 vel = rigidbody2D.velocity;

        targetSpeed = moveDirection * acceleration;
        currentSpeed = targetSpeed;

        amountToMove = new Vector2(currentSpeed, 0);

        if (amountToMove.x != 0)
        {
            vel.x += Mathf.Round(amountToMove.x * Time.deltaTime);

            if (attacking)
            {
                vel.x /= 1.5f;
            }

            if (vel.x < speed && vel.x > -speed)
            {
                rigidbody2D.velocity = vel;
            }
        }

        moveDirection = 0;
    }

    private void UpdateBlocking()
    {
        if (!hasActiveControl)
        {
            return;
        }

        shield.renderer.enabled = false;

        if (blocking)
        {
            shield.renderer.enabled = true;

            if (BlockingCounter > 0)
            {
                blockingCounter -= Time.deltaTime;
                if (BlockingCounter <= 0)
                {
                    blocking = false;
                    blockingCounter = 0;
                    blockingDelayCounter = blockingDelayAmount;
                }

                float scale = BlockingCounter / blockingAmount;

                Vector3 transform = shield.transform.localScale;

                transform.x = scale;
                transform.y = scale;

                shield.transform.localScale = transform;
            }
        }
        else if (blockingDelayCounter > 0)
        {
            blockingDelayCounter -= Time.deltaTime;
            if (blockingDelayCounter <= 0)
            {
                blockingDelayCounter = 0;
            }
        }
        else
        {
            if (BlockingCounter < blockingAmount)
            {
                blockingCounter += Time.deltaTime * 0.8f;
                if (BlockingCounter >= blockingAmount)
                {
                    blockingCounter = blockingAmount;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!hasActiveControl)
        {
            return;
        }

        UpdateMovement();
    }

    // Update is called once per frame
    protected override void Update()
    {
        Score = kills;

        if (IsDead)
        {
            UpdateRespawning();
            return;
        }

#if UNITY_EDITOR
        if (isPrimaryPlayer)
        {
            moveDirection = (int)Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Attack();
            }
        }
#endif

        if (invulnerable)
        {
            UpdateInvulnerability();
        }

        if (((amountToMove.x < 0 && !facingLeft) || (amountToMove.x > 0 && facingLeft)))
        {
            Flip();
        }

        if (punchCounter > 0)
        {
            punchCounter -= Time.deltaTime;
            if (punchCounter < 0)
            {
                punchCounter = 0;
            }
        }

        UpdateBlocking();
        UpdateAnimations();

        //beingHit = false;
        beingBounced = false;

        base.Update();

        if (isMultiplayer && isPrimaryPlayer)
        {
            multiplayerController.SendUpdateMessage(transform.position.x, transform.position.y, rigidbody2D.velocity.x, rigidbody2D.velocity.y, facingLeft, attacking, blocking, health, blockingAmount);
        }
    }

    protected override void Kill()
    {
        base.Kill();

        respawnPhase = 0;
        respawnTimer = 2;

        hasActiveControl = false;

        anim.SetBool("IsHit", false);
        anim.SetBool("IsDead", true);

    }

    protected void Respawn()
    {
        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            if (renderer.gameObject.tag != "Shield" && renderer.color.a <= 0)
            {
                Color color = renderer.color;
                color.a = 1;
                renderer.color = color;
            }
        }

        health = maxHealth;
        hasActiveControl = true;
        isDead = false;
        anim.SetBool("IsDead", false);
        anim.Play("SquirrelIdle");
        EndAttacking();
        beingHit = false;
        wallJump = false;
        jumpCount = 0;
        blocking = false;

        invulnerable = true;
        invulnerableTimer = 2f;
        fadeOut = true;
        fadeTime = 1f / (6f / invulnerableTimer);

        List<GameObject> spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
        GameObject point = spawnPoints[Random.Range(0, spawnPoints.Count)];
        transform.position = point.transform.position;
    }

    private void UpdateInvulnerability()
    {
        if (invulnerableTimer > 0)
        {
            int invisibleCheck = 0;

            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.gameObject.tag != "Shield")
                {
                    Color color = renderer.color;

                    if (fadeOut)
                    {
                        if (color.a <= 0)
                        {
                            invisibleCheck++;
                        }
                        else
                        {
                            color.a -= fadeTime;
                        }
                    }
                    else
                    {
                        if (color.a >= 1)
                        {
                            invisibleCheck++;
                        }
                        else
                        {
                            color.a += fadeTime;
                        }
                    }

                    renderer.color = color;
                }
                else
                {
                    invisibleCheck++;
                }
            }

            if (invisibleCheck == GetComponentsInChildren<SpriteRenderer>().Length)
            {
                fadeOut = !fadeOut;
            }

            invulnerableTimer -= Time.deltaTime;
        }
        else
        {
            invulnerable = false;

            foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if (renderer.gameObject.tag != "Shield")
                {
                    Color color = renderer.color;
                    color.a = 1;
                    renderer.color = color;
                }
            }
        }
    }

    protected void UpdateRespawning()
    {
        if (respawnTimer <= 0)
        {
            if (respawnPhase == 0)
            {
                respawnPhase = 1;
                respawnTimer = 1;
            }
            else if (respawnPhase == 2)
            {
                Respawn();
            }
        }
        else
        {
            if (respawnPhase == 1)
            {
                int invisibleCheck = 0;

                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    if (renderer.gameObject.tag != "Shield" && renderer.color.a > 0)
                    {
                        Color color = renderer.color;
                        color.a -= 0.01f;
                        renderer.color = color;
                    }
                    else
                    {
                        invisibleCheck++;
                    }
                }

                if (invisibleCheck == GetComponentsInChildren<SpriteRenderer>().Length)
                {
                    respawnPhase = 2;
                    respawnTimer = 2;
                }
            }
            else
            {
                respawnTimer -= Time.deltaTime;
            }
        }
    }

    public void UpdateMultiplayerData(float posX, float posY, float velx, float vely, bool facingLeft, bool attacking, bool blocking, float health, float blockingAmount)
    {
        /*Vector3 dest = new Vector3(posX, posY, 0);
        Vector3 vel = Vector3.zero;*/
        transform.position = new Vector3(posX, posY, 0); //Vector3.SmoothDamp(transform.position, dest, ref vel, 0.1f);
        rigidbody2D.velocity = new Vector3(velx, vely, 0);

        if (this.facingLeft != facingLeft)
        {
            Flip();
        }

        this.attacking = attacking;
        this.blocking = blocking;

        this.health = health;
        this.blockingAmount = blockingAmount;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //ContactPoint2D contact = col.contacts[0];

        if (col.gameObject.tag == "SolidTile")
        {
            contactPoints++;

            foreach (ContactPoint2D contact in col.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.red);

                if (contact.normal == Vector2.up)
                {
                    jumpCount = 0;
                    wallJump = false;
                }
                else if (((contact.normal == Vector2.right) ||
                 (contact.normal == -Vector2.right)) && jumpCount > 0)
                {
                    if (((contact.normal == Vector2.right && !facingLeft) || (contact.normal == -Vector2.right && facingLeft)))
                    {
                        Flip();
                    }

                    jumpCount = 0;
                    wallJumpContact.x = contact.normal.x;
                    wallJump = true;
                }
            }
            /*else if (col.gameObject.GetComponent<PlayerController>())
            {
                if (contact.normal == Vector2.up)
                {
                    col.gameObject.GetComponent<PlayerController>().Bounce();
                    jumpCount = 0;
                    Jump();
                }
            }*/
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "SolidTile")
        {
            contactPoints--;
            //ContactPoint2D contact = col.contacts[0];

            foreach (ContactPoint2D contact in col.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.red);

                if (contact.normal == Vector2.up && Mathf.Round(rigidbody2D.velocity.y) != 0)
                {
                    //Player is falling, don't allow jumping.	
                    jumpCount = 1;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Spike")
        {
            if (Health > 0 && !invulnerable)
            {
                TakeDamage(-1);
                invulnerable = true;
                invulnerableTimer = 1.2f;
            }
        }
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing
        facingLeft = !facingLeft;

        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
