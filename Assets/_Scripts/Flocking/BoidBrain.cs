using UnityEngine;

public class BoidBrain : MonoBehaviour
{
    public Vector3 velocity;
    public float maxSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 Cohesion(BoidBrain[] neighbors)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (BoidBrain neighbor in neighbors)
        {
            if (neighbor != this)
            {
                centerOfMass += neighbor.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return (centerOfMass - transform.position).normalized;
        }

        return Vector3.zero;
    }

    public Vector3 Separation(BoidBrain[] neighbors, float separationRadius)
    {
        Vector3 moveAway = Vector3.zero;
        int count = 0;

        foreach (BoidBrain neighbor in neighbors)
        {
            if (neighbor != this && Vector3.Distance(transform.position, neighbor.transform.position) < separationRadius)
            {
                Vector3 difference = transform.position - neighbor.transform.position;
                moveAway += difference.normalized / difference.magnitude; // Key optimization: Inverse scaling
                count++;
            }
        }

        if (count > 0)
        {
            moveAway /= count;
        }

        return moveAway.normalized;
    }

    public Vector3 Alignment(BoidBrain[] neighbors)
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        foreach (BoidBrain neighbor in neighbors)
        {
            if (neighbor != this)
            {
                averageVelocity += neighbor.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            averageVelocity /= count;
            return averageVelocity.normalized;
        }

        return Vector3.zero;
    }
}
