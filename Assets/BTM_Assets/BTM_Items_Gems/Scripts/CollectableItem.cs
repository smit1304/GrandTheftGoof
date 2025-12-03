using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    // Optional: You can also set point value directly on the diamond
    [SerializeField] private int pointValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddScore(pointValue);
            }

            Destroy(gameObject); // remove item after collection
        }
    }
}