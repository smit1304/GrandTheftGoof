using UnityEngine;

public class Diamonds : MonoBehaviour
{
    // Optional: You can also set point value directly on the diamond
    [SerializeField] private int pointValue = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                // Pass the diamond's tag to the inventory
                playerInventory.DiamondCollected(gameObject.tag);
                Destroy(gameObject);
            }
        }
    }
}