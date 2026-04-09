using UnityEngine;
using System.Collections; 

public class PowerUp1 : PowerUp
{
    public float powerUpDuration = 5f; 

    protected override void OnTriggerEnter2D(Collider2D other)
    {
     
        if (other.CompareTag("Player"))
        {
            // Get the Player component
            Player player = other.GetComponent<Player>();

            if (player != null)
            {

                player.ActivateMultiShot();


                StartCoroutine(DeactivateMultiShot(player));
            }

            // Destroy the power-up after collecting it
            Destroy(gameObject);
        }
    }

    private IEnumerator DeactivateMultiShot(Player player)
    {
    
        yield return new WaitForSeconds(powerUpDuration);

 
        player.isMultiShotActive = false;
        Debug.Log("Multi-shot deactivated!");
    }
}