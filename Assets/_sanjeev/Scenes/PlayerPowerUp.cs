using System.Collections;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    [Header("Debug / Info")]
    public bool isPoweredUp = false;
    public float remainingTime = 0f;

    private Coroutine powerUpRoutine;
    
    // Player collects a power up.
    public void ActivatePowerUp(float duration)
    {
        // If already powered up, just refresh the duration.
        if (powerUpRoutine != null)
        {
            StopCoroutine(powerUpRoutine);
        }

        powerUpRoutine = StartCoroutine(PowerUpTimer(duration));
    }

    private IEnumerator PowerUpTimer(float duration)
    {
        isPoweredUp = true;
        remainingTime = duration;

        // Fire global event
        EventManager.PlayerPowerUpStarted();

        while (remainingTime > 0f)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        isPoweredUp = false;
        remainingTime = 0f;

        // Fire global event
        EventManager.PlayerPowerUpEnded();

        powerUpRoutine = null;
    }
}