using UnityEngine;

public class FallDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.RestartLevel();    
        }
    }
}
