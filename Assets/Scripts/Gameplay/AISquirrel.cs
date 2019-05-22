using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]

public class AISquirrel : MonoBehaviour
{
    public enum AIPhase { PathToEnemy, PathToPickup, FreePath, InCombat, BreakingCrate }

    PlayerController playerControl;
    DamagableObject enemyControl;
    Transform targetObject;
    int currentMoveDirection = 0;
    bool block = false;
    //bool freerun = false;
    float blockingTimer = 0;
    float punchDistance;
    float attackCooldown = 0;
    float pickupTimer = 0;

    NodeConnection destinationNode;
    float freerunCounter = 0;
    bool jumpPrepare;
    float breakoutCounter;
    Vector2 breakoutPosition;

    public List<Transform> enemies;
    public NodeManager nodeManager;
    public PickupSpawner pickupSpawner;
    private AIPhase aiPhase;
    private Stack<NodeConnection> navPath;

    // Use this for initialization
    void Start()
    {
        playerControl = GetComponent<PlayerController>();
        punchDistance = playerControl.punchDistance + 0.5f;
        breakoutPosition = new Vector2();

        navPath = new Stack<NodeConnection>();

        GetClosestTarget();
    }

    private void MoveTowardsNode(NodeConnection connection)
    {
        Vector2 destination = connection.node.transform.position;
        Vector2 pos = transform.position;
        Vector2 distanceBetween = new Vector2(Mathf.Abs(destination.x - pos.x), Mathf.Abs(destination.y - pos.y));

        if (distanceBetween.x > 0.1f)
        {
            if (destination.x < pos.x)
            {
                currentMoveDirection = -1;
            }
            else
            {
                currentMoveDirection = 1;
            }
        }

        if (destinationNode.rule == NodeConnection.Rules.WallJump)
        {
            if (playerControl.FacingLeft)
            {
                currentMoveDirection = -1;
            }
            else
            {
                currentMoveDirection = 1;
            }
        }

        if (Vector2.Distance(pos, breakoutPosition) < 0.1f)
        {
            breakoutCounter += Time.deltaTime;
        }

        if (breakoutCounter >= 2)
        {
            breakoutCounter = 0;
            GetClosestTarget();
        }

        breakoutPosition = pos;
    }

    private void GetDestinationPath(Vector2 destinationPosition)
    {
        destinationNode = null;
        navPath = nodeManager.FindPath(nodeManager.GetClosestNodeTo(transform.position), nodeManager.GetClosestNodeTo(destinationPosition));
    }

    private void GetClosestTarget()
    {
        if (playerControl.IsDead)
        {
            return;
        }

        if (Random.Range(0, 100) < 50)
        {
            aiPhase = AIPhase.PathToEnemy;
            enemyControl = enemies[Random.Range(0, enemies.Count)].GetComponentInParent<PlayerController>();
            GetDestinationPath(enemyControl.transform.position);

            return;
        }

        float distance = 100000;
        int target = -1;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].GetComponent<PlayerController>().IsDead && !enemies[i].GetComponent<PlayerController>().Invulnerable)
            {
                float tempDistance = Vector2.Distance(enemies[i].position, rigidbody2D.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    target = i;
                }
            }
        }

        if (target > -1)
        {
            aiPhase = AIPhase.PathToEnemy;
            enemyControl = enemies[target].GetComponentInParent<PlayerController>();
            GetDestinationPath(enemyControl.transform.position);
        }
        else
        {
            enemyControl = null;
        }
    }

    private void GetClosestPickup()
    {
        if (playerControl.IsDead)
        {
            return;
        }

        float distance = 100000;
        int target = -1;
        for (int i = 0; i < pickupSpawner.pickupSpawns.Length; i++)
        {
            if (pickupSpawner.pickupSpawns[i].Pickup != null)// && pickupSpawner.pickupSpawns[i].Pickup.GetComponent<DamagableObject>() == null)
            {
                float tempDistance = Vector2.Distance(pickupSpawner.pickupSpawns[i].Pickup.transform.position, rigidbody2D.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    target = i;
                }
            }
        }

        if (target > -1)
        {
            aiPhase = AIPhase.PathToPickup;
            targetObject = pickupSpawner.pickupSpawns[target].Pickup.transform;
            GetDestinationPath(targetObject.transform.position);
        }
        else
        {
            targetObject = null;
        }
    }

    private void CleanEnemyList()
    {
        if (enemyControl == null || enemyControl.IsDead)
        {
            GetClosestTarget();
        }
    }

    void UpdateOldAI()
    {
        Vector2 enemyPos = enemyControl.transform.position;
        Vector2 myPos = rigidbody2D.position;

        Vector2 distanceBetween = new Vector2(Mathf.Abs(enemyPos.x - myPos.x), Mathf.Abs(enemyPos.y - myPos.y));

        /*if (freerun && !block && attackCooldown == 0)
        {
            if (enemyPos.x > myPos.x)
            {
                currentMoveDirection = -1;
            }
            else
            {
                currentMoveDirection = 1;
            }

            if (Random.Range(0, 100) < 1)
            {
                playerControl.Jump();
            }
        }
        else*/ if (distanceBetween.x > punchDistance - 0.2f && !block && playerControl.CanPunch && attackCooldown == 0)
        {
            if (playerControl.JumpCount == 0)
            {
                if (enemyPos.x < myPos.x)
                {
                    currentMoveDirection = -1;
                }
                else
                {
                    currentMoveDirection = 1;
                }
            }

            if (Random.Range(0, 100) < 1)
            {
                playerControl.Jump();
            }
        }
        /*else if (distanceBetween.x < minimumXDistance && !block && playerControl.CanPunch)
        {
            if (playerControl.JumpCount == 0)
            {
                if (enemyPos.x > myPos.x)
                {
                    currentMoveDirection = -1;
                }
                else
                {
                    currentMoveDirection = 1;
                }
            }
        }*/
        else
        {
            currentMoveDirection = 0;

            if (!block && playerControl.CanPunch && distanceBetween.y <= 0.2f && attackCooldown == 0)
            {
                if (Random.Range(0, 100) < 70)
                {
                    if ((playerControl.FacingLeft && myPos.x < enemyPos.x) ||
                        (!playerControl.FacingLeft && myPos.x > enemyPos.x))
                    {
                        playerControl.Flip();
                    }

                    playerControl.Attack();
                }

                attackCooldown = 1f;
            }
            else if (blockingTimer == 0 && playerControl.CanBlock)
            {
                if (Random.Range(0, 100) < 60)
                {
                    block = true;
                }
                blockingTimer = Random.Range(0.5f, 1f);
            }
            else if (!block)
            {
                if (Random.Range(0, 100) < 40)
                {
                    //freerun = true;
                }
                freerunCounter = Random.Range(0.1f, 0.6f);
            }
        }

        if (blockingTimer > 0)
        {
            blockingTimer -= Time.deltaTime;
            if (blockingTimer <= 0)
            {
                blockingTimer = 0;
                block = false;
            }
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                attackCooldown = 0;
            }
        }

        if (freerunCounter > 0)
        {
            freerunCounter -= Time.deltaTime;
            if (freerunCounter <= 0)
            {
                if (Random.Range(0, 100) < 60)
                {
                    GetClosestTarget();
                }
                freerunCounter = 0;
                //freerun = false;
            }
        }

        if (!playerControl.CanBlock)
        {
            block = false;
        }

        playerControl.MoveDirection = currentMoveDirection;
        playerControl.Blocking = block;
    }

    void ChooseNextPathNode()
    {
        if (navPath != null && navPath.Count > 0)
        {
            destinationNode = navPath.Pop();
        }
        else
        {
            destinationNode = null;
        }

        if (destinationNode == null)
        {

        }
        else if (destinationNode.rule == NodeConnection.Rules.Jump || destinationNode.rule == NodeConnection.Rules.WallJump)
        {
            jumpPrepare = true;
        }
    }

    void UpdatePathToTarget(Vector2 distanceBetween, float minDistance)
    {
        if (distanceBetween.x < minDistance && distanceBetween.y < 0.2f)
        {
            if (aiPhase == AIPhase.PathToEnemy)
            {
                aiPhase = AIPhase.InCombat;
            }
            else if (aiPhase == AIPhase.PathToPickup)
            {
                if (targetObject.gameObject.GetComponent<DamagableObject>() != null)
                {
                    enemyControl = targetObject.gameObject.GetComponent<DamagableObject>();
                    aiPhase = AIPhase.BreakingCrate;
                }
                else
                {
                    GetClosestTarget();
                }
            }
            else if (aiPhase == AIPhase.FreePath)
            {
                if (navPath.Count == 0)
                {
                    GetClosestTarget();
                }
            }
            else
            {
                GetClosestTarget();
            }
        }

        if (jumpPrepare)
        {
            if (playerControl.JumpCount == 0)
            {
                playerControl.Jump();
                jumpPrepare = false;
            }
        }
        else
        {
            if (destinationNode != null)
            {
                MoveTowardsNode(destinationNode);
            }
        }

        if (destinationNode == null || Vector2.Distance(transform.position, destinationNode.node.transform.position) < 0.5f)
        {
            ChooseNextPathNode();
        }
    }

    void UpdateCombat(Vector2 enemyPos, Vector2 myPos, Vector2 distanceBetween)
    {
        if (distanceBetween.x > punchDistance - 0.2f && !block && playerControl.CanPunch && attackCooldown == 0)
        {
            if (playerControl.JumpCount == 0)
            {
                if (enemyPos.x < myPos.x)
                {
                    currentMoveDirection = -1;
                }
                else
                {
                    currentMoveDirection = 1;
                }
            }

            if (Random.Range(0, 100) < 1)
            {
                playerControl.Jump();
            }
        }
        else
        {
            currentMoveDirection = 0;

            if (!block && playerControl.CanPunch && attackCooldown == 0)
            {
                if (distanceBetween.y <= 0.2f)
                {
                    if (Random.Range(0, 100) < 70)
                    {
                        if ((playerControl.FacingLeft && myPos.x < enemyPos.x) ||
                            (!playerControl.FacingLeft && myPos.x > enemyPos.x))
                        {
                            playerControl.Flip();
                        }

                        playerControl.Attack();
                    }

                    attackCooldown = 0.5f;
                }
                else if (distanceBetween.y >= 0.6f)
                {
                    GetDestinationPath(enemyControl.transform.position);
                }
            }
            else if (blockingTimer == 0 && playerControl.CanBlock && enemyControl is PlayerController && !((PlayerController)enemyControl).Blocking)
            {
                if (Random.Range(0, 100) < 70)
                {
                    block = true;
                }
                blockingTimer = Random.Range(0.3f, 0.7f);
            }
            else if (!block)
            {
                if (freerunCounter == 0)
                {
                    if (Random.Range(0, 100) < 40)
                    {
                        ChooseNewFreeRun();
                    }

                    freerunCounter = Random.Range(0.2f, 2.0f);
                }
            }
        }
    }

    void ChooseNewFreeRun()
    {
        aiPhase = AIPhase.FreePath;
        NodeConnection destination = nodeManager.GetClosestNodeTo(transform.position);

        int count = Random.Range(1, 3);

        do
        {
            destination = destination.node.connections[Random.Range(0, destination.node.connections.Count)];
            if (destination.node.canBeDestination)
            {
                count--;
            }
        }
        while (!destination.node.canBeDestination && count != 0);

        destinationNode = null;
        navPath = nodeManager.FindPath(nodeManager.GetClosestNodeTo(transform.position), destination);

        ChooseNextPathNode();
    }

    void UpdateCrateBreaking(Vector2 enemyPos, Vector2 myPos, Vector2 distanceBetween)
    {
        if (enemyControl == null)
        {
            GetClosestTarget();
            return;
        }

        currentMoveDirection = 0;

        if (!block && playerControl.CanPunch && attackCooldown == 0)
        {
            if (distanceBetween.y <= 0.2f)
            {
                if (Random.Range(0, 100) < 70)
                {
                    if ((playerControl.FacingLeft && myPos.x < enemyPos.x) ||
                        (!playerControl.FacingLeft && myPos.x > enemyPos.x))
                    {
                        playerControl.Flip();
                    }

                    playerControl.Attack();
                }

                attackCooldown = 0.5f;
            }
        }
    }

    void UpdateNewAI()
    {
        currentMoveDirection = 0;

        Vector2 targetPos;
        Vector2 myPos = rigidbody2D.position;

        if (aiPhase == AIPhase.PathToPickup)
        {
            if (targetObject == null)
            {
                GetClosestTarget();
                targetPos = enemyControl.transform.position;
            }
            else
            {
                targetPos = targetObject.position;
            }
        }
        else if (aiPhase == AIPhase.FreePath)
        {
            if (destinationNode == null)
            {
                GetClosestTarget();
                targetPos = enemyControl.transform.position;
            }
            else
            {
                targetPos = destinationNode.node.transform.position;
            }
        }
        else
        {
            targetPos = enemyControl.transform.position;
        }

        Vector2 distanceBetween = new Vector2(Mathf.Abs(targetPos.x - myPos.x), Mathf.Abs(targetPos.y - myPos.y));

        switch (aiPhase)
        {
            case AIPhase.FreePath:
            case AIPhase.PathToPickup: UpdatePathToTarget(distanceBetween, 0.2f);
                break;
            case AIPhase.PathToEnemy: UpdatePathToTarget(distanceBetween, punchDistance - 0.2f);
                break;
            case AIPhase.InCombat: UpdateCombat(targetPos, myPos, distanceBetween);
                break;
            case AIPhase.BreakingCrate: UpdateCrateBreaking(targetPos, myPos, distanceBetween);
                break;
        }

        if (aiPhase != AIPhase.PathToPickup && pickupTimer == 0 && pickupSpawner.PickupSpawned())
        {
            if (Random.Range(0, 100) < 30)
            {
                GetClosestPickup();
            }

            pickupTimer = Random.Range(1.0f, 5.0f);
        }

        if (destinationNode == null && navPath.Count == 0)
        {
            GetClosestTarget();
        }

        if (blockingTimer > 0)
        {
            blockingTimer -= Time.deltaTime;
            if (blockingTimer <= 0)
            {
                blockingTimer = 0;
                block = false;
            }
        }

        if (freerunCounter > 0)
        {
            freerunCounter -= Time.deltaTime;
            if (freerunCounter <= 0)
            {
                freerunCounter = 0;
            }
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
            if (attackCooldown <= 0)
            {
                attackCooldown = 0;
            }
        }

        if (pickupTimer > 0)
        {
            pickupTimer -= Time.deltaTime;
            if (pickupTimer <= 0)
            {
                pickupTimer = 0;
            }
        }

        if (!playerControl.CanBlock)
        {
            block = false;
        }

        playerControl.MoveDirection = currentMoveDirection;
        playerControl.Blocking = block;
    }

    // Update is called once per frame
    void Update()
    {
        CleanEnemyList();

        if (playerControl.IsDead || enemyControl == null)
        {
            return;
        }

        /*if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            destinationNode = null;
            navPath = nodeManager.FindPath(nodeManager.GetClosestNodeTo(transform.position), nodeManager.GetClosestNodeTo(Camera.main.ScreenToWorldPoint(/*Input.GetTouch(0).position)));Input.mousePosition)));
        }*/

        //UpdateOldAI();
        UpdateNewAI();
    }
}
