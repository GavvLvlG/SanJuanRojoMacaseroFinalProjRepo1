using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(1);
            }
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
