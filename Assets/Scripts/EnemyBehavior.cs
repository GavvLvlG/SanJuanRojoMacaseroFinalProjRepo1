using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    public enum MovementType { Idle, Wander }
    public enum AttackType { None, Melee, Ranged }

    
    public MovementType movementType = MovementType.Idle;
    public AttackType attackType = AttackType.None;

    public float moveSpeed = 1f;
    public float wanderRadius = 2f;
    public float wanderInterval = 2f; 

    public float attackRate = 1f; 
    public float attackDamage = 5f;
    public float attackRange = 1.5f;

    
    public GameObject enemyBulletPrefab;
    public Transform bulletAttachPoint;
  
    public float bulletSpeed = 5f;

    
    public bool randomizeOnSpawn = true;

    
    public bool rangedAutoFire = true;

   
    public float moveSpeedMin = 0.5f, moveSpeedMax = 2f;
    public float wanderRadiusMin = 0.5f, wanderRadiusMax = 4f;
    public float wanderIntervalMin = 0.5f, wanderIntervalMax = 3f;
    public float attackRateMin = 0.5f, attackRateMax = 3f;
    public float attackDamageMin = 1f, attackDamageMax = 10f;
    public float attackRangeMin = 0.5f, attackRangeMax = 3f;

 
    public Animator animator;
   
    public string animIsMovingParam = "isMoving"; // bool
    public string animAttackTrigger = "Attack"; // trigger

   
    protected Vector3 originPosition;
    protected Vector3 currentTarget;
    protected Coroutine wanderCoroutine;
    protected Coroutine attackCoroutine;


    protected virtual void Awake()
    {
        if (randomizeOnSpawn)
        {
            RandomizeBehavior();
        }
    }

       protected void RandomizeBehavior()
    {
     
        movementType = (Random.value > 0.5f) ? MovementType.Wander : MovementType.Idle;

        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        wanderRadius = Random.Range(wanderRadiusMin, wanderRadiusMax);
        wanderInterval = Random.Range(wanderIntervalMin, wanderIntervalMax);

        
        if (Random.value > 0.5f)
        {
            attackType = (Random.value > 0.5f) ? AttackType.Ranged : AttackType.Melee;
            attackRate = Random.Range(attackRateMin, attackRateMax);
            attackDamage = Random.Range(attackDamageMin, attackDamageMax);
            attackRange = Random.Range(attackRangeMin, attackRangeMax);
        }
        else
        {
            attackType = AttackType.None;
        }
    }

       protected virtual void Start()
    {
        originPosition = transform.position;

        
        SetupMovement();
        SetupAttack();

     
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


        if (Application.isPlaying)
        {
            
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);

            SetupMovement();
            SetupAttack();
        }
    }

    public void InitializeDefault()
    {
      
        if (Application.isPlaying)
        {
            if (wanderCoroutine != null) StopCoroutine(wanderCoroutine);
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);

            SetupMovement();
            SetupAttack();
        }
    }


    public void InitializeSimple(float speed, float radius, float wInterval, bool hasAttack, float aRate, float aDamage, float aRange)
    {

        movementType = (speed > 0f) ? MovementType.Wander : MovementType.Idle;
        moveSpeed = speed;
        wanderRadius = radius;
        wanderInterval = wInterval;

        if (hasAttack)
        {
            attackType = AttackType.Melee; 
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
     
    }

       protected virtual void SetupAttack()
    {
        if (attackCoroutine != null) { StopCoroutine(attackCoroutine); attackCoroutine = null; }

        if (attackType != AttackType.None)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }


    protected virtual IEnumerator WanderRoutine()
    {
      
        while (true)
        {
            currentTarget = originPosition + (Vector3)(Random.insideUnitCircle * wanderRadius);

     
            while (Vector3.Distance(transform.position, currentTarget) > 0.1f)
            {
         
                if (animator != null) animator.SetBool(animIsMovingParam, true);

                transform.position = Vector3.MoveTowards(transform.position, currentTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

 
            if (animator != null) animator.SetBool(animIsMovingParam, false);
            yield return new WaitForSeconds(wanderInterval);
        }
    }


    protected virtual IEnumerator AttackRoutine()
    {
     
        while (true)
        {
   
            if (attackType == AttackType.Ranged && rangedAutoFire)
            {
                DoAttack(null);
            }
            else
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
            }

            yield return new WaitForSeconds(attackRate);
        }
    }

    protected virtual void DoAttack(GameObject target)
    {
       
        switch (attackType)
        {
            case AttackType.Ranged:
                DoRangedAttack(target);
                break;
            case AttackType.Melee:
                DoMeleeAttack(target);
                break;
            case AttackType.None:
            default:

                Debug.LogFormat("{0} has no attack configured (type: {1})", name, attackType);
                break;
        }

        if (animator != null)
        {
       
            animator.SetTrigger(animAttackTrigger);
        }
    }


    protected virtual void DoRangedAttack(GameObject target)
    {
        if (enemyBulletPrefab == null)
        {
            Debug.LogWarningFormat("{0} is configured as Ranged but has no enemyBulletPrefab assigned", name);
            return;
        }

        Transform parent = (bulletAttachPoint != null) ? bulletAttachPoint : transform;

        GameObject bullet = Instantiate(enemyBulletPrefab, parent.position, Quaternion.identity);
        if (bullet == null)
        {
            Debug.LogWarningFormat("{0} failed to instantiate enemyBulletPrefab", name);
            return;
        }

        
        bullet.transform.SetParent(parent, true);

        if (bulletAttachPoint != null)
        {
            bullet.transform.localPosition = Vector3.zero;
        }

        
        bullet.transform.rotation = Quaternion.LookRotation(Vector3.down);
    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    if (rb != null) rb.linearVelocity = Vector3.down * bulletSpeed;
    Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
    if (rb2d != null) rb2d.linearVelocity = Vector2.down * bulletSpeed;

        Debug.LogFormat("{0} fired a projectile downward (parented under {1})", name, parent.name);
    }

    protected virtual void DoMeleeAttack(GameObject target)
    {
        if (target == null)
        {
            Debug.LogFormat("{0} attempted a melee attack but no target was supplied", name);
            return;
        }

        target.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        Debug.LogFormat("{0} attacked {1} for {2} damage (melee)", name, target.name, attackDamage);
    }

    void OnDrawGizmosSelected()
    {
      
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
