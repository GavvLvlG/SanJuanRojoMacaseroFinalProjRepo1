using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public SFXManager SFXManager; // Reference to the SFXManager for playing sound effects
    [Header("Movement")]
    public float speed = 5f;
    private Animator animator;
    private Vector2 lastDirection;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletForce = 20f;
    public Transform firePoint; 

   [Header("Power-Ups")]
public bool isMultiShotActive = false; // Track multi-shot status
public int multiShotCount = 3; // Number of bullets to shoot at once when multi-shot is active
public bool isHomingShotActive = false; // Track homing shot status

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction shootAction;
    private Rigidbody2D rb; 
    private AudioSource audioSource;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found! Please add one to this GameObject.");
            enabled = false; 
            return;
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found! Please add one to this GameObject.");
            enabled = false; 
            return;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");

        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab is not assigned!");
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Fire Point is not assigned! Bullets will be instantiated at the player's position.");
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (shootAction != null && shootAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }

void FixedUpdate()
    {
        Vector2 direction = Vector2.zero;
        if (moveAction != null)
        {
            direction = moveAction.ReadValue<Vector2>();
        }
        Move(direction);

        // Only update walking animation if player is moving
        bool isMoving = direction.sqrMagnitude > 0.01f; // small threshold for floating point error
        if (animator != null)
        {
            animator.SetBool("IsWalk", isMoving);
        }

        if (isMoving)
        {
            // Update lastDirection only when moving
            lastDirection = direction.normalized;
        }

       
    }

    private void Move(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;
        Vector2 velocity = normalizedDirection * speed;
        if (rb != null)
        {
            rb.linearVelocity = velocity;  // Fixed velocity update
        }
    }

    public void Shoot()
{
    if (bulletPrefab != null)
    {
        // Default to player's position if firePoint is not assigned
        Vector3 spawnPosition = (firePoint != null) ? firePoint.position : transform.position;

        int shotCount = isMultiShotActive ? multiShotCount : 1;
        float angleIncrement = 15f;

        for (int i = 0; i < shotCount; i++)
        {
            GameObject bullet;

            if (isHomingShotActive)
            {
                // Instantiate the homing bullet (make sure bulletPrefab is set to Bullet2 or your homing bullet prefab)
                bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                var homingComponent = bullet.GetComponent<Bullet2>();
                if (homingComponent != null)
                {
                    // Initialize any specific properties if needed
                    homingComponent.isHoming = true; // Make sure Bullet2 uses this flag
                }
            }
            else
            {
                // Instantiate normal bullet
                bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    // Determine base direction: prefer firePoint, then lastDirection, then transform.up
                    Vector2 baseDir;
                    if (firePoint != null)
                    {
                        baseDir = firePoint.up;
                    }
                    else if (lastDirection != Vector2.zero)
                    {
                        baseDir = lastDirection;
                    }
                    else
                    {
                        baseDir = transform.up;
                    }

                    float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
                    float angle = baseAngle + (i - (shotCount / 2)) * angleIncrement;
                    Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    bulletRb.linearVelocity = direction.normalized * bulletForce;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
            
            if (SFXManager != null)
            {
                SFXManager.Play("NekoShoot");
            }
        } // end for
    }
    else
    {
        Debug.LogError("Bullet Prefab is not assigned!");
    }
}
    
    public void ActivateMultiShot()
    {
        isMultiShotActive = true;
        Debug.Log("Multi-shot activated!");
       
    }

    public void ActivateHomingShot()
{
    isHomingShotActive = true;
    Debug.Log("Homing shot activated!");
    
}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            if (SFXManager != null)
            {
                SFXManager.Play("NekoDed");
            }
            // Notify the GameManager that the player died so it can show the Game Over UI.
            // Prefer GameManager (centralized), fall back to GameOverManager if present.
            if (GameManager.instance != null)
            {
                GameManager.instance.OnPlayerDeath();
            }
            else if (GameOverManager.instance != null)
            {
                GameOverManager.instance.GameOver();
            }
            else
            {
                Debug.LogError("Player: No GameManager or GameOverManager found to handle game over.");
            }

            // Destroy the player object (keeps behavior consistent with previous implementation)
            Destroy(gameObject);
        }
    }
}