using UnityEngine;


public class EnemyBullet : MonoBehaviour
{
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
       
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
}
