using UnityEngine;

// ChildEnemyBehavior extends EnemyBehavior with a simple "aggro/chase" mode
// Notes:
// - EnemyBehavior's Start and many helpers are declared without access modifiers (private by default).
//   To avoid accidentally hiding Unity's base initialization, this child DOES NOT declare Start()
//   so the base class's Start() will run when this component is used on a GameObject.
// - This script adds a chase behavior that moves the enemy toward the player when within aggroRange.
// - You can attach this script to the same GameObject you would use EnemyBehavior on (or replace the component).

public class ChildEnemyBehavior1 : EnemyBehavior
{
    [Header("Child Behavior (Aggro/Chase)")]
    public float aggroRange = 5f; // distance at which the enemy will start chasing the player
    public float chaseSpeedMultiplier = 1.5f; // multiplier applied to moveSpeed while chasing
    public bool canChase = true; // toggle chasing behavior

    GameObject player;
    bool isChasing = false;

    void Awake()
    {
        // Cache player reference early. If your player uses a different tag, change it in the inspector or here.
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (!canChase || player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist <= aggroRange)
        {
            // Enter chase: move towards player directly. This runs in addition to any wander coroutine
            // started by the base class. That means when chasing, this script will actively move the
            // transform toward the player; when not chasing, the base wander routine will resume.
            isChasing = true;

            float speed = moveSpeed * chaseSpeedMultiplier;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            if (animator != null)
            {
                animator.SetBool(animIsMovingParam, true);
            }
        }
        else
        {
            // Stop chasing and let base movement (e.g., wander) control motion again
            if (isChasing)
            {
                isChasing = false;
                if (animator != null)
                {
                    animator.SetBool(animIsMovingParam, movementType == MovementType.Wander);
                }
            }
        }
    }

    // Small utility to make this enemy stronger during runtime (callable by other scripts)
    public void Enrage(float speedMultiplier, float extraDamage)
    {
        moveSpeed *= speedMultiplier;
        attackDamage += extraDamage;
        TriggerEnrageVisual();
    }

    void TriggerEnrageVisual()
    {
        // Example visual: tint a SpriteRenderer, if present
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
        }
        Debug.LogFormat("{0} enraged: speed={1}, damage={2}", name, moveSpeed, attackDamage);
    }
}
