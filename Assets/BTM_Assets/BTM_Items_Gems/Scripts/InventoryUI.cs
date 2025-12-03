using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Optional Breakdown Display")]
    [SerializeField] private TextMeshProUGUI diamondBreakdownText;
    [SerializeField] private bool showBreakdown = false;


    private void OnEnable()
    {
        
        PlayerInventory.OnInventoryChanged += UpdateDiamondText;
    }

    private void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= UpdateDiamondText;

    }
    public void UpdateDiamondText(int currentScore)
    {
        // Update total score
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString();

        // Optional: Show breakdown of different diamonds collected
        /*if (showBreakdown && diamondBreakdownText != null)
        {
            string breakdown = "Diamonds Collected:\n";
            foreach (var kvp in playerInventory.DiamondCounts)
            {
                breakdown += $"{kvp.Key}: {kvp.Value}\n";
            }
            diamondBreakdownText.text = breakdown;
        }*/
    }
}