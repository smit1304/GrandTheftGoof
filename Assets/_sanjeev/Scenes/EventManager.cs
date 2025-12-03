using System;

public static class EventManager
{
    // -------- POWER UP EVENTS --------
    // Fired when player gets a temporary power up
    public static event Action OnPlayerPowerUpStarted;
    public static event Action OnPlayerPowerUpEnded;

    // --------- Public method to invoke the events---------
    public static void PlayerPowerUpStarted()
    {
        OnPlayerPowerUpStarted?.Invoke();
    }

    public static void PlayerPowerUpEnded()
    {
        OnPlayerPowerUpEnded?.Invoke();
    }
    
}