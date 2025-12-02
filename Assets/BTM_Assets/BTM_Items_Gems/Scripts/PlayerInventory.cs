using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int TotalScore { get; private set; }

    // Dictionary to track different types of diamonds collected
    public Dictionary<string, int> DiamondCounts { get; private set; }

    // Different diamond values
    [System.Serializable]
    public class DiamondValue
    {
        public string diamondTag;
        public int pointValue;
    }

    public List<DiamondValue> diamondValues = new List<DiamondValue>()
    {
        new DiamondValue() { diamondTag = "Coin", pointValue = 1 },
        new DiamondValue() { diamondTag = "GoldBar", pointValue = 10 },
        new DiamondValue() { diamondTag = "RedDiamond", pointValue = 50 }
    };

    public UnityEvent<PlayerInventory> OnDiamondCollected;

    void Start()
    {
        DiamondCounts = new Dictionary<string, int>();

        // Initialize dictionary for all diamond types
        foreach (var diamond in diamondValues)
        {
            DiamondCounts[diamond.diamondTag] = 0;
        }
    }

    public void DiamondCollected(string diamondTag)
    {
        // Find the diamond value
        int value = 0;
        bool diamondFound = false;

        foreach (var diamond in diamondValues)
        {
            if (diamond.diamondTag == diamondTag)
            {
                value = diamond.pointValue;
                diamondFound = true;
                break;
            }
        }

        // If tag not found in list, use default value
        if (!diamondFound)
        {
            Debug.LogWarning($"Diamond tag '{diamondTag}' not found in diamondValues. Using default value of 10.");
            value = 10;

            // Add to dictionary if not present
            if (!DiamondCounts.ContainsKey(diamondTag))
            {
                DiamondCounts[diamondTag] = 0;
            }
        }

        // Update counts and total score
        TotalScore += value;

        if (DiamondCounts.ContainsKey(diamondTag))
        {
            DiamondCounts[diamondTag]++;
        }
        else
        {
            DiamondCounts[diamondTag] = 1;
        }

        OnDiamondCollected.Invoke(this);
    }
}