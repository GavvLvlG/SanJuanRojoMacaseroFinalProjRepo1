using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, MultiShot, HomingShot }

    public PowerUpType powerUpType; 

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}