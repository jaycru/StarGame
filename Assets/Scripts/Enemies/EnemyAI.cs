using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： pjjay
//功能说明：人机行为逻辑
//***************************************** 
public class EnemyAI : MonoBehaviour
{
    [Header("基础设置")]
    public Transform player;
    public float moveSpeed = 3.5f;
    public float rotationSpeed = 10f;

    [Header("视野")]
    public float viewRadius = 10f;
    public float viewAngle = 60f;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("攻击")]
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.0f;

    [Header("呼叫支援")]
    public float callForHelpRadius = 15f;

    private float currentAttackCooldown = 0f;
    private bool isChasing = false;
    private float currentViewRadius;

    enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        currentViewRadius = viewRadius;
    }

    void Update()
    {
        if (player == null) return;

        bool isPlayerInView = CheckIfPlayerInView();

        switch (currentState)
        {
            case EnemyState.Idle:
                if (isPlayerInView)
                {
                    StartChase();
                }
                break;

            case EnemyState.Chase:
                if (isPlayerInView)
                {
                    MoveTowardsPlayer();

                    float distToPlayer = Vector3.Distance(transform.position, player.position);
                    if (distToPlayer <= attackRange)
                    {
                        currentState = EnemyState.Attack;
                    }
                }
                else
                {
                    currentState = EnemyState.Idle;
                }
                break;

            case EnemyState.Attack:
                if (isPlayerInView && Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    AttackPlayer();
                }
                else
                {
                    currentState = EnemyState.Chase;
                }
                break;
        }
    }

    bool CheckIfPlayerInView()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer > currentViewRadius) return false;

        Vector3 directionToPlayer = player.position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle / 2f) return false;

        if (Physics.Linecast(transform.position, player.position, obstacleLayer))
        {
            return false;
        }

        return true;
    }

    void MoveTowardsPlayer()
    {
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void StartChase()
    {
        currentState = EnemyState.Chase;
        CallNearbyEnemies();
    }

    void CallNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, callForHelpRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.gameObject != this.gameObject)
            {
                EnemyAI otherEnemy = hit.GetComponent<EnemyAI>();
                if (otherEnemy != null)
                {
                    if (otherEnemy.currentState == EnemyState.Idle)
                    {
                        otherEnemy.StartChase();
                    }
                }
            }
        }
        Debug.DrawRay(transform.position, Vector3.up * 2, Color.yellow, 0.5f);
    }
    void AttackPlayer()
    {
        if (currentAttackCooldown <= 0)
        {
            Debug.Log($"敌人{gameObject.name}攻击了玩家！造成{attackDamage}点伤害");
            //调用玩家扣血脚本
            //改动：改动PlayerHealth为EnemyHealth
            EnemyHealth playerCtrl = player.GetComponent<EnemyHealth>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(attackDamage);
            }

            currentAttackCooldown = attackCooldown;
        }
        else
        {
            currentAttackCooldown -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.yellow;
        Vector3 direction = transform.forward;
        Quaternion leftAngle = Quaternion.Euler(0, -viewAngle / 2, 0);
        Quaternion rightAngle = Quaternion.Euler(0, viewAngle / 2, 0);

        Gizmos.DrawRay(transform.position, leftAngle * direction * viewRadius);
        Gizmos.DrawRay(transform.position, rightAngle * direction * viewRadius);
    }
}
