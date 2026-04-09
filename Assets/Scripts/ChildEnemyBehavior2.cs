using UnityEngine;


public class ChildEnemyBehavior2 : EnemyBehavior
{
    [Header("Child Behavior (Sideways Movement)")]
    public float horizontalAmplitude = 1.5f;
    public float horizontalFrequency = 0.5f; 

    float initialX = float.NaN;

    void Update()
    {
        if (float.IsNaN(initialX)) initialX = transform.position.x;

        float newX = initialX + Mathf.Sin(Time.time * Mathf.PI * 2f * horizontalFrequency) * horizontalAmplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }


    public void Enrage(float speedMultiplier, float extraDamage)
    {
        moveSpeed *= speedMultiplier;
        attackDamage += extraDamage;
        TriggerEnrageVisual();
    }

    void TriggerEnrageVisual()
    {
        
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
        }
        Debug.LogFormat("{0} enraged: speed={1}, damage={2}", name, moveSpeed, attackDamage);
    }
}
