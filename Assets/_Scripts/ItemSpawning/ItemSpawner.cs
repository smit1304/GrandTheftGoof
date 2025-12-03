using System.Collections;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Time between each item spawn (in seconds)")]
    public float spawnInterval = 2f;   // default rate: 1 item every 2 seconds

    [Header("Spawn Locations")]
    public Transform[] spawnPoints;

    [Header("Items to Spawn (Weight-Based)")]
    public ItemSpawnData[] items;

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("ItemSpawner: No spawn points assigned!");
            return;
        }
        if (items.Length == 0)
        {
            Debug.LogError("ItemSpawner: No items assigned!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnItem();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnItem()
    {
        if (spawnPoints.Length == 0 || items.Length == 0)
        {
            Debug.LogWarning("Spawner missing spawn points or items.");
            return;
        }

        // Pick random spawn position
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Pick item based on probability
        GameObject selectedItem = GetRandomWeightedItem();

        if (selectedItem != null)
            Instantiate(selectedItem, spawnPoint.position, spawnPoint.rotation);
    }

    GameObject GetRandomWeightedItem()
    {
        float total = 0f;
        foreach (var i in items)
            total += i.probability;

        float roll = Random.value * total;
        float cumulative = 0f;

        foreach (var i in items)
        {
            cumulative += i.probability;
            if (roll <= cumulative)
                return i.prefab;
        }

        return null; // shouldn't happen unless all probs = 0
    }
}
