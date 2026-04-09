using UnityEngine;
using System.Collections;

public class PowerUp2 : PowerUp
{
    public float powerUpDuration = 5f; 

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                
                player.ActivateHomingShot();

   
                StartCoroutine(DeactivateHomingShot(player));
            }

            Destroy(gameObject);
        }
    }

    private IEnumerator DeactivateHomingShot(Player player)
    {
        yield return new WaitForSeconds(powerUpDuration);
        player.isHomingShotActive = false;
        Debug.Log("Homing shot deactivated!");
    }
}