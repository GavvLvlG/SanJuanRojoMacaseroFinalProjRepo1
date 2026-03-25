using UnityEngine;
using System.Collections; // Add this line

public class PowerUp1 : PowerUp
{
    public float powerUpDuration = 5f; // Duration of the power-up effect

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Get the Player component
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // Activate the multi-shot power-up
                player.ActivateMultiShot();

                // Optionally, deactivate the power-up after a certain duration
                StartCoroutine(DeactivateMultiShot(player));
            }

            // Destroy the power-up after collecting it
            Destroy(gameObject);
        }
    }

    private IEnumerator DeactivateMultiShot(Player player)
    {
        // Wait for the duration of the power-up
        yield return new WaitForSeconds(powerUpDuration);

        // Optionally, you can add a function to deactivate the power-up
        // For example, resetting isMultiShotActive to false after the duration
        player.isMultiShotActive = false;
        Debug.Log("Multi-shot deactivated!");
    }
}