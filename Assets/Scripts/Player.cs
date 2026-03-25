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
        Vector3 spawnPosition = transform.position;
        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }

        // Determine shooting direction
        Vector2 shootDirection = Vector2.up;
        if (firePoint != null)
        {
            shootDirection = new Vector2(firePoint.up.x, firePoint.up.y);
        }

        // Shoot multiple bullets if multi-shot is active
        int shotCount = isMultiShotActive ? multiShotCount : 1;
        float angleIncrement = 15f; // Spread angle for multi-shot

        for (int i = 0; i < shotCount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            if (bulletRb != null)
            {
                // Calculate spread angles for multiple shots
                float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg + (i - (shotCount / 2)) * angleIncrement;
                Vector2 spreadDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                // Apply velocity in the chosen direction
                bulletRb.linearVelocity = spreadDirection.normalized * bulletForce;

                // Rotate the bullet to face the travel direction
                bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                Debug.LogError("Bullet prefab is missing a Rigidbody2D component!");
                Destroy(bullet);
            }
        }
    }
    else
    {
        Debug.LogError("Bullet Prefab is not assigned!");
    }
}

    // Call this function to activate multi-shot power-up
    public void ActivateMultiShot()
    {
        isMultiShotActive = true;
        Debug.Log("Multi-shot activated!");
        // Optionally, you can add a timer to deactivate multi-shot after some time
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