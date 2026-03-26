using UnityEngine;

public class Bullet1 : Bullet
{
    [Header("Multi-shot settings")]
    public int multiShotCount = 3;
    public float spreadAngle = 30f; 
    
    public bool isChild = false;

    private void Start()
    {
        if (!isChild && multiShotCount > 1)
        {
            float baseAngle = transform.eulerAngles.z; // Use current rotation or default to 0
            float half = spreadAngle / 2f;

            for (int i = 0; i < multiShotCount; i++)
            {
                float angle;
                if (multiShotCount == 1)
                    angle = baseAngle;
                else
                    angle = baseAngle - half + (spreadAngle / (multiShotCount - 1)) * i;

                float rad = angle * Mathf.Deg2Rad;
                Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

                GameObject clone = Instantiate(gameObject, transform.position, Quaternion.Euler(0, 0, angle));
                Bullet1 cloneScript = clone.GetComponent<Bullet1>();
                if (cloneScript != null)
                {
                    cloneScript.isChild = true;
                    cloneScript.multiShotCount = 1; 
                    // No need to set direction anymore
                }
            }

            Destroy(gameObject);
            return;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}