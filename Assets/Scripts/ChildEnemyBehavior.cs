using UnityEngine;


public class ChildEnemyBehavior : EnemyBehavior
{
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
