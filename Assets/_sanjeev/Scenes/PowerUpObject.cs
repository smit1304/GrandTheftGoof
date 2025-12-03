using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpObject : MonoBehaviour
{
    [Header("Power Up Settings")]
    public float powerUpDuration = 5f;     // seconds of power

    [Header("Pickup Settings")]
    public bool destroyOnPickup = true;    // destroy this object after pickup

    private void Reset()
    {
        // Make sure the collider is a trigger by default
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react to player
        if (!other.CompareTag("Player"))
            return;

        // Does the player have a PlayerPowerUp component?
        PlayerPowerUp playerPower = other.GetComponent<PlayerPowerUp>();
        if (playerPower == null)
        {
            Debug.LogWarning("Player collided with PowerUp but has no PlayerPowerUp component!");
            return;
        }

        // Activate power up on the player
        playerPower.ActivatePowerUp(powerUpDuration);

        // Destroy the picked up item
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            // Or disable it
            gameObject.SetActive(false);
        }
    }
}