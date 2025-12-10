using UnityEngine;
using UnityEngine.AI;

public class TargetNavmesh : MonoBehaviour
{
    private NavMeshAgent[] navAgents;
    public Transform targetMarker;
    public float verticalOffset = 10.0f;
    void Start()
    {
        navAgents = FindObjectsOfType(typeof(NavMeshAgent)) as NavMeshAgent[];
    }
    void UpdateTargets(Vector3 targetPosition)
    {
        foreach (NavMeshAgent agent in navAgents)
        {
            agent.destination = targetPosition;
        }
    }
    void Update()
    {
        // Get the point of the hit position when the mouse is being clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;
                UpdateTargets(targetPosition);
                targetMarker.position = targetPosition + new Vector3(0,
                verticalOffset, 0);
            }
        }
    }

}
