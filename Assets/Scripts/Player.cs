using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public AudioManager audioManager;

    [Header("Movement")]
    public float speed = 5f;

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
        if (shootAction.WasPressedThisFrame())
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();
        Move(direction);
    }

    private void Move(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;
        Vector2 velocity = normalizedDirection * speed;
        rb.linearVelocity = velocity;  // Fixed velocity update
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
                    float angle = Mathf.Atan2(firePoint.up.y, firePoint.up.x) * Mathf.Rad2Deg + (i - (shotCount / 2)) * angleIncrement;
                    Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                    bulletRb.linearVelocity = direction.normalized * bulletForce;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
        }
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
            audioManager.Play("NekoDed");
            Destroy(gameObject);
        }
    }
}