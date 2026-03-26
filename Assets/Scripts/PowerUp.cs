using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Enum to manage different power-up types
    public enum PowerUpType { SpeedBoost, MultiShot, HomingShot }

    public PowerUpType powerUpType; // The type of power-up this is

    // Protected virtual method to be overridden in derived classes
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // This can be used in the derived class for specific logic (like activating power-ups)
    }
}