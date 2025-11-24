using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockingNavMeshManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int flockSize = 20;
    public Transform[] waypoints;
    private int currentWaypoint = 0;

    [Header("Flock Properties")]
    public float cohesionWeight = 1f; 
    public float separationWeight = 1f; 
    public float alignmentWeight = 1f;
    public float separationRadius = 2f;

    [Range(0,1)]
    public float speedDebuffMultiplier;
    public BoidNavMeshBrain[] allBoids;

    void Start()
    {
        allBoids = new BoidNavMeshBrain[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            Vector3 spawnPos = Vector3.zero;
            bool found = false;

            // Keep searching until a valid NavMesh point is found
            int attempts = 0;
            while (!found && attempts < 30) // safety limit to avoid infinite loop
            {
                Vector3 randomPos = transform.position + Random.insideUnitSphere * 10;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPos, out hit, 10f, NavMesh.AllAreas))
                {
                    spawnPos = hit.position;
                    found = true;
                }

                attempts++;
            }

            if (found)
            {
                GameObject newBoid = Instantiate(boidPrefab, spawnPos, Quaternion.identity);
                allBoids[i] = newBoid.GetComponent<BoidNavMeshBrain>();
            }
            else
            {
                Debug.LogWarning("Boid " + i + " could not find a valid NavMesh spawn point.");
            }

        }

        SetGroupDestination(waypoints[currentWaypoint].position);
    }

    void Update()
    {
        // Check if flock leader (or average position) reached destination
        Vector3 flockCenter = Vector3.zero;
        foreach (var boid in allBoids) flockCenter += boid.transform.position;
        flockCenter /= allBoids.Length;

        if (Vector3.Distance(flockCenter, waypoints[currentWaypoint].position) < 2f)
        {
            currentWaypoint = Random.Range(0, waypoints.Length);
            SetGroupDestination(waypoints[currentWaypoint].position);
        }

        foreach (var boid in allBoids)
        {
            var neighbors = FindNeighbors(boid, 5f);
            boid.UpdateBoid(neighbors, speedDebuffMultiplier, cohesionWeight, separationWeight, alignmentWeight, separationRadius);
        }
    }

    void SetGroupDestination(Vector3 dest)
    {
        foreach (var boid in allBoids)
        {
            boid.SetDestination(dest);
        }
    }

    BoidNavMeshBrain[] FindNeighbors(BoidNavMeshBrain boid, float radius)
    {
        //Naive implementation, REPLACE WITH SPATIAL PARTITIONING FOR PERFORMANCE
        List<BoidNavMeshBrain> neighbors = new List<BoidNavMeshBrain>();
        foreach (BoidNavMeshBrain otherBoid in allBoids)
        {
            if (otherBoid != boid && Vector3.Distance(boid.transform.position, otherBoid.transform.position) < radius)
            {
                neighbors.Add(otherBoid);
            }
        }
        return neighbors.ToArray();
    }


}
