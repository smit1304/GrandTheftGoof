using UnityEngine;
using UnityEngine.AI;

public class BoidNavMeshBrain : MonoBehaviour
{
    public float maxSpeed = 5f;
    public Vector3 velocity;
    private NavMeshAgent agent;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // we’ll handle rotation manually
        agent.updatePosition = false; // we’ll move via boid logic
    }

    public void SetDestination(Vector3 dest)
    {
        agent.SetDestination(dest);
    }

    public void UpdateBoid(BoidNavMeshBrain[] neighbors,float speedDebuffMultiplier, float cohesionWeight, float separationWeight, float alignmentWeight, float separationRadius)
    {
        // NavMesh desired velocity
        Vector3 navVel = agent.desiredVelocity;

        // Flocking forces
        Vector3 cohesion = Cohesion(neighbors) * cohesionWeight;
        Vector3 separation = Separation(neighbors, separationRadius) * separationWeight;
        Vector3 alignment = Alignment(neighbors) * alignmentWeight;

        // Blend
        velocity += navVel + cohesion + separation + alignment;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed) * speedDebuffMultiplier;

        // Apply movement
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);

        // Sync agent position
        agent.nextPosition = transform.position;

        // Update animation
        anim.SetFloat("Walk", agent.velocity.magnitude);
    }


    public Vector3 Cohesion(BoidNavMeshBrain[] neighbors)
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (BoidNavMeshBrain neighbor in neighbors)
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

    public Vector3 Separation(BoidNavMeshBrain[] neighbors, float separationRadius)
    {
        Vector3 moveAway = Vector3.zero;
        int count = 0;

        foreach (BoidNavMeshBrain neighbor in neighbors)
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

    public Vector3 Alignment(BoidNavMeshBrain[] neighbors)
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        foreach (BoidNavMeshBrain neighbor in neighbors)
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
