using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public enum MovementType { Idle, Wander }
    public enum AttackType { None, Melee, Ranged }

    // Public state (editable or set at runtime)
    public MovementType movementType = MovementType.Idle;
    public AttackType attackType = AttackType.None;

    public float moveSpeed = 1f;
    public float wanderRadius = 2f;
    public float wanderInterval = 2f; // time between choosing new wander targets

    public float attackRate = 1f; // seconds between attacks
    public float attackDamage = 5f;
    public float attackRange = 1.5f;

    Vector3 originPosition;
    Vector3 currentTarget;
    Coroutine wanderCoroutine;
    Coroutine attackCoroutine;

    void Start()
    {
        originPosition = transform.position;

        // Start behavior coroutines according to configured types
        SetupMovement();
        SetupAttack();
    }

    public void Initialize(MovementType mType, AttackType aType, float speed, float radius, float wInterval, float aRate, float aDamage, float aRange)
    {
        movementType = mType;
        attackType = aType;
        moveSpeed = speed;
        wanderRadius = radius;
        wanderInterval = wInterval;
        attackRate = aRate;
        attackDamage = aDamage;
        attackRange = aRange;

        // If Start already ran, restart coroutines with new settings
        if (Application.isPlaying)
        {
            // stop existing coroutines if any
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);

            SetupMovement();
            SetupAttack();
        }
    }

    public void InitializeDefault()
    {
        // No-op default initializer; ensures coroutines are started with defaults
        if (Application.isPlaying)
        {
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);

            SetupMovement();
            SetupAttack();
        }
    }

    // Lightweight initializer used by the simplified spawner
    public void InitializeSimple(float speed, float radius, float wInterval, bool hasAttack, float aRate, float aDamage, float aRange)
    {
        // Default to wander movement when a speed is provided
        movementType = (speed > 0f) ? MovementType.Wander : MovementType.Idle;
        moveSpeed = speed;
        wanderRadius = radius;
        wanderInterval = wInterval;

        if (hasAttack)
        {
            attackType = AttackType.Melee; // simple default
            attackRate = aRate;
            attackDamage = aDamage;
            attackRange = aRange;
        }
        else
        {
            attackType = AttackType.None;
        }

        if (Application.isPlaying)
        {
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);

            SetupMovement();
            SetupAttack();
        }
    }

    void SetupMovement()
    {
        if (wanderCoroutine != null) { StopCoroutine(wanderCoroutine); wanderCoroutine = null; }

        if (movementType == MovementType.Wander)
        {
            // Start wandering coroutine
            wanderCoroutine = StartCoroutine(WanderRoutine());
        }
        // Idle needs no coroutine
    }

    void SetupAttack()
    {
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); attackCoroutine = null; }

        if (attackType != AttackType.None)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    IEnumerator WanderRoutine()
    {
        // Basic wander: pick a random point around the origin within radius, move to it using MoveTowards, then wait
        while (true)
        {
            currentTarget = originPosition + (Vector3)(Random.insideUnitCircle * wanderRadius);

            // move until close to target
            while (Vector3.Distance(transform.position, currentTarget) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // wait before choosing next wander target
            yield return new WaitForSeconds(wanderInterval);
        }
    }

    IEnumerator AttackRoutine()
    {
        // Attack loop: attempt attack every attackRate seconds if player is in range
        while (true)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist <= attackRange)
                {
                    DoAttack(player);
                }
            }

            yield return new WaitForSeconds(attackRate);
        }
    }

    void DoAttack(GameObject target)
    {
        // Placeholder attack behavior: send a message to the target if it implements a damage handler
        // This keeps things decoupled; no error if the method doesn't exist
        target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

        // For visibility during testing, log the attack
        Debug.LogFormat("{0} attacked {1} for {2} damage (type: {3})", name, target.name, attackDamage, attackType);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize wander radius and attack range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
