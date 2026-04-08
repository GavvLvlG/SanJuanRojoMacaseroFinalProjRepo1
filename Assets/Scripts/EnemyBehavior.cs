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

    // Animator to play animations on the enemy (assign in inspector)
    public Animator animator;
    // Names for animator parameters (change to match your Animator)
    public string animIsMovingParam = "isMoving"; // bool
    public string animAttackTrigger = "Attack"; // trigger

    // Protected so child classes can read/manipulate these when extending behavior
    protected Vector3 originPosition;
    protected Vector3 currentTarget;
    protected Coroutine wanderCoroutine;
    protected Coroutine attackCoroutine;

    // Made virtual so child classes can override initialization (call base.Start() if overriding)
    protected virtual void Start()
    {
        originPosition = transform.position;

        // Start behavior coroutines according to configured types
        SetupMovement();
        SetupAttack();

        // Initialize animator parameters to match starting state
        if (animator != null)
        {
            animator.SetBool(animIsMovingParam, movementType == MovementType.Wander);
        }
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

    // Allow children to customize movement setup
    protected virtual void SetupMovement()
    {
        if (wanderCoroutine != null) { StopCoroutine(wanderCoroutine); wanderCoroutine = null; }

        if (movementType == MovementType.Wander)
        {
            // Start wandering coroutine
            wanderCoroutine = StartCoroutine(WanderRoutine());
            if (animator != null) animator.SetBool(animIsMovingParam, true);
        }
        else
        {
            if (animator != null) animator.SetBool(animIsMovingParam, false);
        }
        // Idle needs no coroutine
    }

    // Allow children to customize attack setup
    protected virtual void SetupAttack()
    {
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); attackCoroutine = null; }

        if (attackType != AttackType.None)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    // Virtual so children can replace wandering behavior
    protected virtual IEnumerator WanderRoutine()
    {
        // Basic wander: pick a random point around the origin within radius, move to it using MoveTowards, then wait
        while (true)
        {
            currentTarget = originPosition + (Vector3)(Random.insideUnitCircle * wanderRadius);

            // move until close to target
            while (Vector3.Distance(transform.position, currentTarget) > 0.1f)
            {
                // Ensure the moving animation is active while moving
                if (animator != null) animator.SetBool(animIsMovingParam, true);

                transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // wait before choosing next wander target
            // we're idle while waiting for next target
            if (animator != null) animator.SetBool(animIsMovingParam, false);
            yield return new WaitForSeconds(wanderInterval);
        }
    }

    // Virtual so children can replace attack behavior
    protected virtual IEnumerator AttackRoutine()
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

    // Virtual so child classes can implement different attack effects
    protected virtual void DoAttack(GameObject target)
    {
        // Placeholder attack behavior: send a message to the target if it implements a damage handler
        // This keeps things decoupled; no error if the method doesn't exist
        target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);

        // Trigger attack animation if available
        if (animator != null)
        {
            // Use trigger so animation plays once per attack
            animator.SetTrigger(animAttackTrigger);
        }

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
