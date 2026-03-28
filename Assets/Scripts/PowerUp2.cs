using UnityEngine;
using System.Collections;

public class PowerUp2 : PowerUp
{
    public float powerUpDuration = 5f; // Duration of the power-up effect

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                // Activate the homing shot power-up
                player.ActivateHomingShot();

                // Deactivate after duration
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