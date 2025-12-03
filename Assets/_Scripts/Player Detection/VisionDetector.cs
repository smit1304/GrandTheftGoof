using System;
using System.Collections;
using UnityEngine;

public class VisionDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    public Transform target;          // usually your player
    public float viewRadius = 10f;    // how far they can see
    [Range(0, 360)]
    public float viewAngle = 90f;     // cone angle (like 90° vision)

    public LayerMask obstacleMask;    // walls/obstacles
    public LayerMask targetMask;      // player layer

    public static Action OnPlayerSpotted;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found!");
        }

       
        if (playerObj != null)
            target = playerObj.transform;

        StartCoroutine(VisionRoutine());

    }
    

    bool CanSeeTarget()
    {
        if (target == null) return false;

        // 1. Distance check
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distToTarget = Vector3.Distance(transform.position, target.position);
        if (distToTarget > viewRadius) return false;

        // 2. Angle check
        if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2f) return false;

        // 3. Line of sight check
        // Cast a ray toward the target, but only check against obstacles
        if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
        {
            // No obstacle in the way → target is visible
            OnPlayerSpotted?.Invoke();
            Debug.Log(gameObject.name + " can see the player");
            return true;
        }

        Debug.Log(gameObject.name + " cannot see the player");
        return false;

    }

    // Optional: visualize the vision cone in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);
    }

    private IEnumerator VisionRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(0.2f); // 5 checks/sec

        while (!GameManager.isGameOver)
        {
            if (CanSeeTarget())
            {
                Debug.Log(name + " sees the player!");
            }

            yield return delay;
        }
    }
}
