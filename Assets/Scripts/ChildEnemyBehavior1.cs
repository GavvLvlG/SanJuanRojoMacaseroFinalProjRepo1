using UnityEngine;



public class ChildEnemyBehavior1 : EnemyBehavior
{
    [Header("Child Behavior (Vertical Movement)")]
    public float verticalAmplitude = 1.5f; // how far from the start Y the enemy moves
    public float verticalFrequency = 0.5f; // cycles per second

    float initialY = float.NaN;

    void Update()
    {

        if (float.IsNaN(initialY)) initialY = transform.position.y;


        float newY = initialY + Mathf.Sin(Time.time * Mathf.PI * 2f * verticalFrequency) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
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
