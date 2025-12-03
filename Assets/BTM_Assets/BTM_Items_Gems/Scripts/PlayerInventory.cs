using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class PlayerInventory : MonoBehaviour
{
    public int TotalScore { get; private set; }

    public static Action<int> OnInventoryChanged;

    public void AddScore(int amount)
    {
        TotalScore += amount;

        // Notify UI or other systems
        OnInventoryChanged?.Invoke(TotalScore);

        Debug.Log("Player Score: " + TotalScore);
    }
}