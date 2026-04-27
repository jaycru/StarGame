using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class EnemyAI : MonoBehaviour
{
    // --- 状态枚举 ---
    enum AIState { Patrol, Chase }
    AIState currentState = AIState.Patrol;

    // --- 组件引用 ---
    private CharacterController controller;
    private Transform playerTransform;

    // --- 参数设置 ---
    [Header("侦测设置")]
    public float detectRange = 15f;
    public float viewAngle = 90f;

    [Header("移动速度")]
    public float patrolSpeed = 5f;
    public float chaseSpeed = 20f;

    [Header("巡逻设置")]
    public float patrolRadius = 45f;
    public float stopDistance = 1f;
    public float rotationSpeed = 10f;

    [Header("近战攻击")]
    public float attackRange = 2f;
    public float attackInterval = 1f;
    public int attackDamage = 10;

    // --- 巡逻变量 ---
    private Vector3 homePosition;
    private Vector3 targetPosition;

    // --- 重力相关 ---
    public float gravity = -9.81f;
    private Vector3 velocity;

    // --- 攻击状态 ---
    private bool isAttacking;
    private Coroutine meleeAttackCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        homePosition = transform.position;
        SetNewRandomTarget();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Debug.Log("playerObj is :" + playerObj.name);
            playerTransform = playerObj.transform;
        }
    }

    void Update()
    {
        CheckForPlayer();

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolLogic();
                break;
            case AIState.Chase:
                ChaseLogic();
                break;
        }

        ApplyGravity();
    }

    private void OnDisable()
    {
        StopMeleeAttack();
    }

    void CheckForPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectRange)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.y = 0;

            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < viewAngle / 2)
            {
                currentState = AIState.Chase;
            }
        }
        else
        {
            currentState = AIState.Patrol;
            StopMeleeAttack();
        }
    }

    void PatrolLogic()
    {
        StopMeleeAttack();
        MoveTowards(targetPosition, patrolSpeed);

        // 注意：这里只判断水平距离，防止高度差导致无法到达
        Vector3 flatPos = transform.position;
        Vector3 flatTarget = targetPosition;
        flatPos.y = 0;
        flatTarget.y = 0;

        if (Vector3.Distance(flatPos, flatTarget) < stopDistance)
        {
            SetNewRandomTarget();
        }
    }

    void ChaseLogic()
    {
        if (playerTransform == null)
        {
            StopMeleeAttack();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        Debug.Log("距离玩家: " + distanceToPlayer);

        // 已进入攻击状态时，保持静止（只朝向玩家，不执行追击移动）
        if (isAttacking)
        {
            FaceTarget(playerTransform.position);

            // 玩家脱离攻击范围则结束攻击，下一帧恢复追击
            if (distanceToPlayer > attackRange)
            {
                StopMeleeAttack();
            }
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            FaceTarget(playerTransform.position);
            meleeAttackCoroutine = StartCoroutine(MeleeAttackCoroutine());
        }
        else
        {
            MoveTowards(playerTransform.position, chaseSpeed);
        }
    }

    IEnumerator MeleeAttackCoroutine()
    {
        isAttacking = true;
        WaitForSeconds wait = new WaitForSeconds(attackInterval);

        while (currentState == AIState.Chase && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > attackRange)
            {
                break;
            }

            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.TakeDamage(attackDamage);
                Debug.Log(name + " 发起近战攻击，造成伤害: " + attackDamage);
            }

            yield return wait;
        }

        isAttacking = false;
        meleeAttackCoroutine = null;
    }

    void StopMeleeAttack()
    {
        if (meleeAttackCoroutine != null)
        {
            StopCoroutine(meleeAttackCoroutine);
            meleeAttackCoroutine = null;
        }
        isAttacking = false;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void SetNewRandomTarget()
    {
        // --- 修正部分 ---
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;

        // randomPoint.x 对应 X轴
        // randomPoint.y 对应 Z轴 (之前写成了 randomPoint.z 所以 Z 轴不动)
        targetPosition = new Vector3(
            homePosition.x + randomPoint.x,
            transform.position.y, // 保持当前高度，防止摔倒或悬空
            homePosition.z + randomPoint.y
        );
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            controller.Move(direction.normalized * speed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (velocity.y < 0)
            {
                velocity.y = -0.5f;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, detectRange);

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, attackRange);

        if (Application.isPlaying)
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Vector3 center = new Vector3(homePosition.x, transform.position.y, homePosition.z);
            Gizmos.DrawSphere(center, patrolRadius);
        }
    }
}