using UnityEngine;

public class Bullet2 : Bullet
{
    [Header("Homing settings")]
    public bool isHoming = true; // Set to true for homing behavior
    public float homingSpeed = 5f; // Speed at which the bullet steers
    public float rotateSpeed = 200f; // How fast the bullet can turn
    private Transform target;

    private void Start()
    {
        if (isHoming)
        {
            // Find the closest enemy as the target
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy.transform;
                }
            }
        }
        // Remove multi-shot logic
    }

    private void Update()
    {
        if (isHoming && target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float rotateAmount = Vector3.Cross(transform.up, direction).z;

            // Rotate towards the target
            transform.Rotate(0, 0, -rotateAmount * rotateSpeed * Time.deltaTime);

            // Move forward
            transform.Translate(Vector3.up * homingSpeed * Time.deltaTime);
        }
        else
        {
            // Move straight if no target or not homing
            transform.Translate(Vector3.up * homingSpeed * Time.deltaTime);
        } // <-- Missing brace added here
    } // <-- Added here

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}